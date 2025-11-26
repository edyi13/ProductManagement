using MediatR;
using ProductManagement.Application.Common.Models;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>
{
}