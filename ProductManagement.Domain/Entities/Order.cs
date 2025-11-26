using ProductManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        // bussiness rules should live inside the entity in the domain layer
        public decimal CalculateTotal()
        { 
            return OrderItems.Sum(item => item.Quantity * item.UnitPrice);
        }
    }
}
