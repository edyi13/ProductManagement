using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application.Common.Interfaces;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Messaging;

namespace ProductManagement.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            // Register DbContext with PostgreSQL
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure RabbitMQ settings
            services.Configure<RabbitMQSettings>(
                configuration.GetSection("RabbitMQ"));

            // Register RabbitMQ Publisher as Singleton
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

            return services;
        }
    }
}
