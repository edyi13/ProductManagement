using ProductManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Interfaces
{
    //Dependency Inversion Principle
    //- High-level modules should not depend
    //on low-level modules. Both should depend on abstractions.
    public interface ICategoryRepository: IRepository<Category>
    {
        Task<Category> GetCategoryWithProductsAsync(int categoryId);
    }
}
