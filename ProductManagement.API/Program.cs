using ProductManagement.API.Middleware;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.DependencyInjection;
using ProductManagement.Application.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Use AddSwaggerGen (not AddOpenApi)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "Clean Architecture API with CQRS, Repository Pattern, and RabbitMQ"
    });
});

// Add Application Layer (MediatR, FluentValidation, Behaviors)
builder.Services.AddApplication();

// Add Infrastructure Layer (DbContext, Repositories, UnitOfWork, RabbitMQ)
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS (optional, but recommended for frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline
app.UseExceptionMiddleware(); // Custom exception handler (must be first!)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();