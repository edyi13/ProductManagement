using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryQuery : IRequest<Result<IEnumerable<ProductDto>>>
{
    public int CategoryId { get; set; }
}