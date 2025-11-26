using MediatR;
using ProductManagement.Application.Common.Models;

namespace ProductManagement.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<int>>
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
