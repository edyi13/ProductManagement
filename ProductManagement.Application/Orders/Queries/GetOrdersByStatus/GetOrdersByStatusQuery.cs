using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Orders.Queries.GetOrdersByStatus;

public class GetOrdersByStatusQuery : IRequest<Result<IEnumerable<OrderDto>>>
{
    public string Status { get; set; }
}