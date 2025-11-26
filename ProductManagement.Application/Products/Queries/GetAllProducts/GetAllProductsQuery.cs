using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Interfaces;

namespace ProductManagement.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>>>
{
}