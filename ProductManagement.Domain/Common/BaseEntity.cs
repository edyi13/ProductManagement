using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Common
{
    //following the DRY principle, this base entity class
    //can be inherited by other entity classes to include
    //common properties like Id, CreatedAt, and UpdatedAt.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
