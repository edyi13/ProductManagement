using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.Data.Repositories
{
    public class CategoryRepository: Repository<Category> ,ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<Category> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
