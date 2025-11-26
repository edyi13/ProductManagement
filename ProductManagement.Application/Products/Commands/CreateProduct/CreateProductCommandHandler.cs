using MediatR;
using ProductManagement.Application.Common.Interfaces;
using ProductManagement.Application.Common.Models;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Events;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler: IRequestHandler<CreateProductCommand, Result<int>>
    {
        IUnitOfWork _unitOfWork;
        IMessagePublisher _messagePublisher;
        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMessagePublisher message)
        {
            _unitOfWork = unitOfWork;
            _messagePublisher = message;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // check if category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return Result<int>.Failure($"Category with ID {request.CategoryId} not found");
            }
            // create product
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            // add product
            await _unitOfWork.Products.AddAsync(product);
            // save changes
            await  _unitOfWork.SaveChangesAsync();
            // publish event
            // Publish event to RabbitMQ
            var productCreatedEvent = new ProductCreatedEvent
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price
            };
            await _messagePublisher.PublishAsync(productCreatedEvent, "product-events");
            return Result<int>.Success(product.Id);
        }
    }
}
