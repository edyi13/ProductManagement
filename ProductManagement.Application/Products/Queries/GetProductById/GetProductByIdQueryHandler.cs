using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // retrieve product by id
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        //check if product exists
        if (product == null)
        {
            return Result<ProductDto>.Failure($"Product with ID {request.Id} not found");
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "Unknown",
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };

        return Result<ProductDto>.Success(productDto);
    }
}