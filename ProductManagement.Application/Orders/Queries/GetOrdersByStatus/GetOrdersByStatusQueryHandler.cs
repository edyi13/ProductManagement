using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Orders.Queries.GetOrdersByStatus;

public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, Result<IEnumerable<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrdersByStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(request.Status);

        //TODO validate if no orders found

        var orderDtos = orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            Items = o.OrderItems?.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Subtotal
            }).ToList() ?? new List<OrderItemDto>()
        }).ToList();

        return Result<IEnumerable<OrderDto>>.Success(orderDtos);
    }
}