using MediatR;
using ProductManagement.Application.Common.Interfaces;
using ProductManagement.Application.Common.Models;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Events;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher)
    {
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
    }

    public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate all products exist and have sufficient stock
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = new Dictionary<int, Product>();

        foreach (var productId in productIds)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return Result<int>.Failure($"Product with ID {productId} not found");
            }
            products[productId] = product;
        }

        // Check stock availability
        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            if (product.Stock < item.Quantity)
            {
                return Result<int>.Failure($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {item.Quantity}");
            }
        }

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create order
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Create order items and reduce stock
            var orderItems = new List<OrderItem>();
            var lowStockProducts = new List<Product>();

            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];

                // Reduce stock
                product.Stock -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Products.Update(product);

                // Create order item
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = product.Price * item.Quantity
                };

                await _unitOfWork.OrderItems.AddAsync(orderItem);
                orderItems.Add(orderItem);

                // Check for low stock
                if (product.Stock < 10)
                {
                    lowStockProducts.Add(product);
                }
            }

            // Calculate total
            order.TotalAmount = orderItems.Sum(oi => oi.Subtotal);
            _unitOfWork.Orders.Update(order);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Publish OrderCreatedEvent
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                OrderDate = order.CreatedAt
            };
            await _messagePublisher.PublishAsync(orderCreatedEvent, "product-events");

            // Publish LowStockEvents
            foreach (var product in lowStockProducts)
            {
                var lowStockEvent = new LowStockEvent
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CurrentStock = product.Stock
                };
                await _messagePublisher.PublishAsync(lowStockEvent, "product-events");
            }

            return Result<int>.Success(order.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result<int>.Failure($"Failed to create order: {ex.Message}");
        }
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}