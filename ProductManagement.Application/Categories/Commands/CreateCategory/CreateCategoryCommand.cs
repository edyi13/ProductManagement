using MediatR;
using ProductManagement.Application.Common.Models;

namespace ProductManagement.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}