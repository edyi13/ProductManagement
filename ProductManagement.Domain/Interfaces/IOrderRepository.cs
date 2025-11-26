using ProductManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Interfaces
{
    public interface IOrderRepository: IRepository<Order>
    {
        Task<Order> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<Order> GetOrderByOrderNumberAsync(string orderNumber);
    }
}
