using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<Result<OrderDto>>
{
    public int Id { get; set; }
}
