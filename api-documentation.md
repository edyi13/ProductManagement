# Product Management API - Complete Documentation

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Database Schema](#database-schema)
5. [API Endpoints](#api-endpoints)
6. [Setup Instructions](#setup-instructions)
7. [Configuration](#configuration)
8. [Design Patterns](#design-patterns)
9. [Message Queue Integration](#message-queue-integration)
10. [Testing](#testing)

---

## 🎯 Project Overview

**Product Management API** is a production-ready RESTful API built with Clean Architecture principles. It manages products, categories, and orders with full CRUD operations, async messaging via RabbitMQ, and robust data persistence.

**Key Features:**
- ✅ Clean Architecture (4-layer separation)
- ✅ CQRS Pattern (Command Query Responsibility Segregation)
- ✅ Repository & Unit of Work Pattern
- ✅ RabbitMQ Integration for event-driven architecture
- ✅ PostgreSQL Database with EF Core
- ✅ FluentValidation for input validation
- ✅ MediatR for request/response handling
- ✅ Swagger/OpenAPI documentation
- ✅ Async/await throughout for scalability

---

## 🏗️ Architecture

### Clean Architecture Layers

```
ProductManagement/
├── ProductManagement.API/              # Presentation Layer
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
│
├── ProductManagement.Application/      # Application Layer (Use Cases)
│   ├── Products/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Orders/
│   ├── Categories/
│   ├── DTOs/
│   └── Common/
│
├── ProductManagement.Domain/           # Domain Layer (Business Logic)
│   ├── Entities/
│   ├── Interfaces/
│   └── Events/
│
└── ProductManagement.Infrastructure/   # Infrastructure Layer
    ├── Data/
    ├── Repositories/
    ├── Messaging/
    └── DependencyInjection/
```

### Layer Dependencies
```
API → Application → Domain
  ↓
Infrastructure → Application + Domain
```

**Key Principle:** Domain has no dependencies. Application depends only on Domain. Infrastructure implements interfaces defined in Domain/Application.

---

## 💻 Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | .NET | 8.0/9.0 |
| Language | C# | 11.0+ |
| Database | PostgreSQL | 15+ |
| ORM | Entity Framework Core | 9.0.0 |
| Message Queue | RabbitMQ | 3.x |
| API Documentation | Swagger/Swashbuckle | 6.x |
| Validation | FluentValidation | 11.x |
| Mediator | MediatR | 12.x |
| Logging | Serilog | (optional) |

---

## 🗄️ Database Schema

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductManagementDB;Username=postgres;Password=your_password"
  }
}
```

### Tables

#### **1. categories**
Stores product categories.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | SERIAL | PRIMARY KEY | Auto-increment ID |
| Name | VARCHAR(200) | NOT NULL | Category name |
| Description | TEXT | NULLABLE | Category description |
| IsActive | BOOLEAN | DEFAULT TRUE | Active status |
| CreatedAt | TIMESTAMP | DEFAULT NOW() | Creation timestamp |
| UpdatedAt | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- Primary Key: `Id`

---

#### **2. products**
Stores product information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | SERIAL | PRIMARY KEY | Auto-increment ID |
| Name | VARCHAR(200) | NOT NULL | Product name |
| Description | TEXT | NULLABLE | Product description |
| Price | NUMERIC(18,2) | NOT NULL | Product price |
| Stock | INTEGER | NOT NULL, DEFAULT 0 | Available stock |
| CategoryId | INTEGER | NOT NULL, FK | Foreign key to categories |
| IsActive | BOOLEAN | DEFAULT TRUE | Active status |
| CreatedAt | TIMESTAMP | DEFAULT NOW() | Creation timestamp |
| UpdatedAt | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Foreign Keys:**
- `CategoryId` → `categories(Id)` ON DELETE RESTRICT

**Indexes:**
- Primary Key: `Id`
- Index: `CategoryId`
- Index: `Name`

---

#### **3. orders**
Stores customer orders.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | SERIAL | PRIMARY KEY | Auto-increment ID |
| OrderNumber | VARCHAR(50) | UNIQUE, NOT NULL | Unique order number |
| CustomerName | VARCHAR(200) | NOT NULL | Customer full name |
| CustomerEmail | VARCHAR(200) | NOT NULL | Customer email |
| TotalAmount | NUMERIC(18,2) | NOT NULL | Total order amount |
| Status | VARCHAR(50) | NOT NULL, DEFAULT 'Pending' | Order status |
| CreatedAt | TIMESTAMP | DEFAULT NOW() | Creation timestamp |
| UpdatedAt | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Valid Status Values:**
- `Pending`
- `Processing`
- `Shipped`
- `Completed`
- `Cancelled`

**Indexes:**
- Primary Key: `Id`
- Unique Index: `OrderNumber`
- Index: `Status`

---

#### **4. orderitems**
Stores individual items within orders.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | SERIAL | PRIMARY KEY | Auto-increment ID |
| OrderId | INTEGER | NOT NULL, FK | Foreign key to orders |
| ProductId | INTEGER | NOT NULL, FK | Foreign key to products |
| Quantity | INTEGER | NOT NULL | Quantity ordered |
| UnitPrice | NUMERIC(18,2) | NOT NULL | Price per unit at time of order |
| Subtotal | NUMERIC(18,2) | NOT NULL | Quantity × UnitPrice |

**Foreign Keys:**
- `OrderId` → `orders(Id)` ON DELETE CASCADE
- `ProductId` → `products(Id)` ON DELETE RESTRICT

**Indexes:**
- Primary Key: `Id`
- Index: `OrderId`
- Index: `ProductId`

---

### Entity Relationships

```
categories (1) ←──→ (*) products
products (1) ←──→ (*) orderitems
orders (1) ←──→ (*) orderitems
```

**Relationship Details:**
- One Category has many Products
- One Product can be in many OrderItems
- One Order has many OrderItems
- Deleting a Category is restricted if it has Products
- Deleting an Order cascades to OrderItems
- Deleting a Product is restricted if it's in OrderItems

---

### Database Creation Script

```sql
-- Create Database
CREATE DATABASE ProductManagementDB;

-- Connect to database
\c ProductManagementDB

-- Create tables (created automatically by EF Core migrations)
-- Or run: dotnet ef database update

-- Verify tables
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public'
ORDER BY table_name;
```

---

## 🔌 API Endpoints

### Base URL
```
https://localhost:{port}/api
```

---

### **Categories Endpoints**

#### 1. Get All Categories
```http
GET /api/categories
```

**Response 200 OK:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Electronics",
      "description": "Electronic devices and accessories",
      "isActive": true,
      "productCount": 15
    }
  ],
  "errorMessage": null,
  "errors": []
}
```

#### 2. Create Category
```http
POST /api/categories
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Electronics",
  "description": "Electronic devices and accessories"
}
```

**Response 201 Created:**
```json
{
  "isSuccess": true,
  "data": 1,
  "errorMessage": null,
  "errors": []
}
```

**Validation Rules:**
- `name`: Required, max 200 characters
- `description`: Optional, max 1000 characters

---

### **Products Endpoints**

#### 1. Get All Products
```http
GET /api/products
```

**Response 200 OK:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Gaming Laptop",
      "description": "RTX 4060, 16GB RAM, 512GB SSD",
      "price": 1499.99,
      "stock": 25,
      "categoryId": 2,
      "categoryName": "Computers",
      "isActive": true,
      "createdAt": "2025-11-26T10:30:00Z"
    }
  ],
  "errorMessage": null,
  "errors": []
}
```

#### 2. Get Product by ID
```http
GET /api/products/{id}
```

**Response 200 OK:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "Gaming Laptop",
    "description": "RTX 4060, 16GB RAM, 512GB SSD",
    "price": 1499.99,
    "stock": 25,
    "categoryId": 2,
    "categoryName": "Computers",
    "isActive": true,
    "createdAt": "2025-11-26T10:30:00Z"
  },
  "errorMessage": null,
  "errors": []
}
```

**Response 404 Not Found:**
```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Product not found",
  "errors": ["Product not found"]
}
```

#### 3. Get Products by Category
```http
GET /api/products/category/{categoryId}
```

**Response:** Same structure as Get All Products

#### 4. Create Product
```http
POST /api/products
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Gaming Laptop",
  "description": "RTX 4060, 16GB RAM, 512GB SSD",
  "price": 1499.99,
  "stock": 25,
  "categoryId": 2
}
```

**Response 201 Created:**
```json
{
  "isSuccess": true,
  "data": 1,
  "errorMessage": null,
  "errors": []
}
```

**Validation Rules:**
- `name`: Required, max 200 characters
- `description`: Optional, max 1000 characters
- `price`: Required, must be > 0
- `stock`: Required, must be ≥ 0
- `categoryId`: Required, must exist in database

**Events Published:**
- `ProductCreatedEvent` → RabbitMQ queue

#### 5. Update Product
```http
PUT /api/products/{id}
Content-Type: application/json
```

**Request Body:**
```json
{
  "id": 1,
  "name": "Gaming Laptop Pro",
  "description": "RTX 4070, 32GB RAM, 1TB SSD",
  "price": 1999.99,
  "stock": 30,
  "categoryId": 2
}
```

**Response 204 No Content**

**Validation:** Same as Create Product + ID must match route parameter

#### 6. Delete Product
```http
DELETE /api/products/{id}
```

**Response 204 No Content**

**Business Rule:** Cannot delete product if it exists in any orders

**Response 400 Bad Request (if in orders):**
```json
{
  "isSuccess": false,
  "data": 0,
  "errorMessage": "Cannot delete product that exists in orders",
  "errors": ["Cannot delete product that exists in orders"]
}
```

---

### **Orders Endpoints**

#### 1. Get Order by ID
```http
GET /api/orders/{id}
```

**Response 200 OK:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "orderNumber": "ORD-20251126-ABC123",
    "customerName": "John Smith",
    "customerEmail": "john@example.com",
    "totalAmount": 1549.98,
    "status": "Pending",
    "createdAt": "2025-11-26T10:30:00Z",
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Gaming Laptop",
        "quantity": 1,
        "unitPrice": 1499.99,
        "subtotal": 1499.99
      },
      {
        "id": 2,
        "productId": 6,
        "productName": "Wireless Mouse",
        "quantity": 1,
        "unitPrice": 49.99,
        "subtotal": 49.99
      }
    ]
  },
  "errorMessage": null,
  "errors": []
}
```

#### 2. Get Orders by Status
```http
GET /api/orders/status/{status}
```

**Valid Status Values:**
- `Pending`
- `Processing`
- `Shipped`
- `Completed`
- `Cancelled`

**Response:** Array of orders with same structure as Get Order by ID

#### 3. Create Order
```http
POST /api/orders
Content-Type: application/json
```

**Request Body:**
```json
{
  "customerName": "John Smith",
  "customerEmail": "john@example.com",
  "items": [
    {
      "productId": 1,
      "quantity": 1
    },
    {
      "productId": 6,
      "quantity": 2
    }
  ]
}
```

**Response 201 Created:**
```json
{
  "isSuccess": true,
  "data": 1,
  "errorMessage": null,
  "errors": []
}
```

**Validation Rules:**
- `customerName`: Required, max 200 characters
- `customerEmail`: Required, valid email format
- `items`: Required, must have at least 1 item
- Each item: `productId` > 0, `quantity` > 0

**Business Logic:**
1. Validates all products exist
2. Checks sufficient stock for each product
3. Reduces stock quantities
4. Generates unique order number (format: `ORD-{timestamp}-{guid}`)
5. Calculates subtotals and total
6. Uses database transaction (all or nothing)
7. Publishes `OrderCreatedEvent` to RabbitMQ
8. Publishes `LowStockEvent` if any product stock < 10

**Response 400 Bad Request (insufficient stock):**
```json
{
  "isSuccess": false,
  "data": 0,
  "errorMessage": "Insufficient stock for product: Gaming Laptop",
  "errors": ["Insufficient stock for product: Gaming Laptop"]
}
```

---

## 🚀 Setup Instructions

### Prerequisites
- .NET SDK 8.0 or 9.0
- PostgreSQL 15+
- RabbitMQ 3.x (or Docker)
- Visual Studio 2022 / VS Code / Rider

### Step 1: Clone Repository
```bash
git clone <repository-url>
cd ProductManagement
```

### Step 2: Setup PostgreSQL
```bash
# Install PostgreSQL
# Create database
psql -U postgres
CREATE DATABASE ProductManagementDB;
\q
```

### Step 3: Setup RabbitMQ
```bash
# Option 1: Docker (recommended)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Option 2: Install locally from rabbitmq.com
```

### Step 4: Configure Connection Strings
Edit `ProductManagement.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductManagementDB;Username=postgres;Password=YOUR_PASSWORD"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "product-events"
  }
}
```

### Step 5: Restore Packages
```bash
dotnet restore
```

### Step 6: Run Database Migrations
```bash
dotnet ef database update --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

### Step 7: Run the Application
```bash
dotnet run --project ProductManagement.API
```

### Step 8: Access Swagger
```
https://localhost:5001/swagger
```

### Step 9: Insert Sample Data
Run the sample data SQL script provided in the documentation.

---

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductManagementDB;Username=postgres;Password=your_password"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "product-events"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables (Production)
```bash
export ConnectionStrings__DefaultConnection="Host=prod-server;Database=ProductManagementDB;..."
export RabbitMQ__HostName="rabbitmq-server"
export RabbitMQ__UserName="prod-user"
export RabbitMQ__Password="prod-password"
```

---

## 🎨 Design Patterns

### 1. Clean Architecture
**Purpose:** Separation of concerns, testability, independence from frameworks

**Layers:**
- **Domain:** Business entities and rules (no dependencies)
- **Application:** Use cases (CQRS commands/queries)
- **Infrastructure:** Data access, external services
- **API:** Controllers, middleware, DI configuration

### 2. CQRS (Command Query Responsibility Segregation)
**Purpose:** Separate read and write operations for optimization and clarity

**Commands (Write):**
- `CreateProductCommand`
- `UpdateProductCommand`
- `DeleteProductCommand`
- `CreateOrderCommand`
- `CreateCategoryCommand`

**Queries (Read):**
- `GetAllProductsQuery`
- `GetProductByIdQuery`
- `GetProductsByCategoryQuery`
- `GetOrderByIdQuery`
- `GetOrdersByStatusQuery`
- `GetAllCategoriesQuery`

### 3. Repository Pattern
**Purpose:** Abstract data access logic, provide collection-like interface

**Implementation:**
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
```

**Specific Repositories:**
- `IProductRepository` - Custom product queries
- `ICategoryRepository` - Category with products
- `IOrderRepository` - Orders with items

### 4. Unit of Work Pattern
**Purpose:** Coordinate multiple repositories in a single transaction

**Implementation:**
```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

**Usage Example:**
```csharp
// All changes saved in single transaction
var product = await _unitOfWork.Products.GetByIdAsync(1);
product.Price = 999.99;
_unitOfWork.Products.Update(product);

var category = await _unitOfWork.Categories.GetByIdAsync(2);
category.Name = "Updated Name";
_unitOfWork.Categories.Update(category);

await _unitOfWork.SaveChangesAsync(); // Commits both changes
```

### 5. Result Pattern
**Purpose:** Explicit error handling without exceptions for business rule violations

**Implementation:**
```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
    public List<string> Errors { get; set; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
```

### 6. Mediator Pattern (MediatR)
**Purpose:** Decouple request senders from handlers

**Flow:**
```
Controller → MediatR → Handler → Repository → Database
```

### 7. Dependency Injection
**Purpose:** Loose coupling, testability

**Registration (Program.cs):**
```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
```

---

## 🐰 Message Queue Integration

### RabbitMQ Setup

**Queue Name:** `product-events`

**Exchange:** Direct (default)

**Events Published:**

#### 1. ProductCreatedEvent
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "occurredOn": "2025-11-26T10:30:00Z",
  "productId": 1,
  "productName": "Gaming Laptop",
  "price": 1499.99
}
```

**Triggered:** When product is created via POST /api/products

#### 2. OrderCreatedEvent
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "occurredOn": "2025-11-26T10:30:00Z",
  "orderId": 1,
  "orderNumber": "ORD-20251126-ABC123",
  "totalAmount": 1549.98,
  "orderDate": "2025-11-26T10:30:00Z"
}
```

**Triggered:** When order is created via POST /api/orders

#### 3. LowStockEvent
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "occurredOn": "2025-11-26T10:30:00Z",
  "productId": 1,
  "productName": "Gaming Laptop",
  "currentStock": 5
}
```

**Triggered:** When product stock falls below 10 units after order creation

### Consumer Example (External Service)

```csharp
var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "product-events",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received: {message}");
    
    // Process event
    var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(message);
    // Handle the event...
};

channel.BasicConsume(
    queue: "product-events",
    autoAck: true,
    consumer: consumer
);
```

---

## 🧪 Testing

### Unit Testing Commands

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Test Structure

```
ProductManagement.Tests/
├── Application/
│   ├── Products/
│   │   ├── CreateProductCommandHandlerTests.cs
│   │   └── GetProductByIdQueryHandlerTests.cs
│   └── Orders/
│       └── CreateOrderCommandHandlerTests.cs
├── Domain/
│   └── Entities/
│       └── OrderTests.cs
└── Infrastructure/
    └── Repositories/
        └── ProductRepositoryTests.cs
```

### Sample Test

```csharp
[Fact]
public async Task CreateProduct_ValidInput_ReturnsSuccess()
{
    // Arrange
    var command = new CreateProductCommand
    {
        Name = "Test Product",
        Price = 99.99m,
        Stock = 10,
        CategoryId = 1
    };
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.True(result.Data > 0);
}
```

---

## 📊 Performance Considerations

### Database Optimization
- Indexes on frequently queried columns (CategoryId, OrderNumber, Status)
- Async operations throughout
- Connection pooling via EF Core
- Use `AsNoTracking()` for read-only queries

### API Best Practices
- Async/await for all I/O operations
- DTOs to prevent over-fetching
- Pagination for large datasets (implement as needed)
- Response caching (implement as needed)

### RabbitMQ
- Durable queues for reliability
- Message acknowledgment
- Connection pooling
- Batch publishing for high volume

---

## 🔒 Security Considerations

### Current Implementation
- Input validation via FluentValidation
- SQL injection prevention via parameterized queries (EF Core)
- HTTPS enforced in production

### Recommended Additions
- JWT authentication
- Role-based authorization
- Rate limiting
- API key management
- CORS configuration
- Input sanitization

---

## 📝 Maintenance

### Database Migrations

**Create Migration:**
```bash
dotnet ef migrations add MigrationName --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

**Apply Migration:**
```bash
dotnet ef database update --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

**Rollback Migration:**
```bash
dotnet ef database update PreviousMigrationName --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

### Monitoring
- Check RabbitMQ Management UI: http://localhost:15672
- Monitor PostgreSQL connections
- Application logs via Serilog (if configured)
- Health check endpoints (implement as needed)

---

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Write unit tests for new features
3. Use async/await for I/O operations
4. Follow naming conventions
5. Update documentation

---

## 📄 License

MIT License - See LICENSE file for details

---

## 👥 Support

For questions or issues:
- Check Swagger documentation at `/swagger`
- Review this documentation
- Check application logs
- Verify RabbitMQ connection
- Verify database connection

---

**Last Updated:** November 26, 2025
**Version:** 1.0.0
**Author:** Development Team