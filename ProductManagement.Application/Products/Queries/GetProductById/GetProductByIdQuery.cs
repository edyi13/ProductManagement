using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public int Id { get; set; }
}