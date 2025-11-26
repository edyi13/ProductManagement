using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Events
{
    public class BaseDomainEvents
    {
        Guid Id { get; set; }
        DateTime OccurredOn { get; set; }

        public BaseDomainEvents()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
