using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.Requests;
using ProductManagement.API.Models.Responses;
using ProductManagement.Application.Products.Commands.CreateProduct;
using ProductManagement.Application.Products.Commands.UpdateProduct;
using ProductManagement.Application.Products.Commands.DeleteProduct;
using ProductManagement.Application.Products.Queries.GetAllProducts;
using ProductManagement.Application.Products.Queries.GetProductById;
using ProductManagement.Application.Products.Queries.GetProductsByCategory;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAllProducts()
        {
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = result.Data.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(response, "Products retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(ApiResponse<ProductResponse>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = new ProductResponse
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                Price = result.Data.Price,
                Stock = result.Data.Stock,
                CategoryId = result.Data.CategoryId,
                CategoryName = result.Data.CategoryName,
                IsActive = result.Data.IsActive,
                CreatedAt = result.Data.CreatedAt
            };

            return Ok(ApiResponse<ProductResponse>.SuccessResponse(response, "Product retrieved successfully"));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetProductsByCategory(int categoryId)
        {
            var query = new GetProductsByCategoryQuery { CategoryId = categoryId };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = result.Data.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(response, "Products retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateProduct(CreateProductRequest request)
        {
            var command = new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<int>.ErrorResponse(result.ErrorMessage, result.Errors));

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = result.Data },
                ApiResponse<int>.SuccessResponse(result.Data, "Product created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateProduct(int id, UpdateProductRequest request)
        {
            var command = new UpdateProductCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse.ErrorResponse(result.ErrorMessage, result.Errors));

            return Ok(ApiResponse.SuccessResponse("Product updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(int id)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse.ErrorResponse(result.ErrorMessage, result.Errors));

            return NoContent();
        }
    }
}
