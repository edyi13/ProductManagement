using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<IEnumerable<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsByCategoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(request.CategoryId);

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