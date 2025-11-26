using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetAllAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Unknown",
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        }).ToList();

        return Result<IEnumerable<ProductDto>>.Success(productDtos);
    }
}