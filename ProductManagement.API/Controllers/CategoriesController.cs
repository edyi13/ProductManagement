using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.Requests;
using ProductManagement.API.Models.Responses;
using ProductManagement.Application.Categories.Commands.CreateCategory;
using ProductManagement.Application.Categories.Queries.GetAllCategories;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponse>>>> GetAllCategories()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<IEnumerable<CategoryResponse>>.ErrorResponse(result.ErrorMessage, result.Errors));

            var response = result.Data.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ProductCount = c.ProductCount
            }).ToList();

            return Ok(ApiResponse<IEnumerable<CategoryResponse>>.SuccessResponse(response, "Categories retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateCategory(CreateCategoryRequest request)
        {
            var command = new CreateCategoryCommand
            {
                Name = request.Name,
                Description = request.Description
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<int>.ErrorResponse(result.ErrorMessage, result.Errors));

            return CreatedAtAction(
                nameof(GetAllCategories),
                null,
                ApiResponse<int>.SuccessResponse(result.Data, "Category created successfully"));
        }
    }
}
