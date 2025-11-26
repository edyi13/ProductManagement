using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.Common.Interfaces
{
    // Interface for publishing messages to RabbitMQ
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queueName) where T : class;
    }
}
