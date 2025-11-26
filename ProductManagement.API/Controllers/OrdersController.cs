using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.Requests;
using ProductManagement.API.Models.Responses;
using ProductManagement.Application.Orders.Commands.CreateOrder;
using ProductManagement.Application.Orders.Queries.GetOrderById;
using ProductManagement.Application.Orders.Queries.GetOrdersByStatus;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrderById(int id)
        {
            var query = new GetOrderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(ApiResponse<OrderResponse>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = new OrderResponse
            {
                Id = result.Data.Id,
                OrderNumber = result.Data.OrderNumber,
                CustomerName = result.Data.CustomerName,
                CustomerEmail = result.Data.CustomerEmail,
                TotalAmount = result.Data.TotalAmount,
                Status = result.Data.Status,
                CreatedAt = result.Data.CreatedAt,
                Items = result.Data.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            };

            return Ok(ApiResponse<OrderResponse>.SuccessResponse(response, "Order retrieved successfully"));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponse>>>> GetOrdersByStatus(string status)
        {
            var query = new GetOrdersByStatusQuery { Status = status };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = result.Data.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            }).ToList();

            return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(response, "Orders retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateOrder(CreateOrderRequest request)
        {
            var command = new CreateOrderCommand
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Items = request.Items.Select(i => new CreateOrderCommand.OrderItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<int>.ErrorResponse(result.ErrorMessage, result.Errors));

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = result.Data },
                ApiResponse<int>.SuccessResponse(result.Data, "Order created successfully"));
        }
    }
}
