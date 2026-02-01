# FreshNoodle Architecture Guide - Understanding the Workflow

## Introduction

This guide explains how the FreshNoodle application is structured and how data flows through the different layers. We'll use the **Product** feature as an example to understand the complete workflow.

## The Big Picture

FreshNoodle follows a **Clean Architecture** pattern with clear separation of concerns. Think of it like a restaurant:

- **UI (Blazor)** = The dining room where customers interact
- **API Controller** = The waiter taking orders and serving food
- **Service Layer** = The kitchen manager coordinating the cooking
- **Repository Layer** = The chef preparing the actual food
- **Database** = The pantry where ingredients are stored

## Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                    BROWSER (User)                            │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (HTTPS/JSON API Calls)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│              LAYER 1: UI (Blazor WebAssembly)                │
│  Files: FreshNoodle.UI/Pages/Products.razor                   │
│  Responsibilities:                                           │
│  - Display data to user                                      │
│  - Capture user input (forms)                                │
│  - Call API services                                         │
│  - Handle navigation                                         │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (HTTP Requests/Responses)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│         LAYER 2: UI Service (API Client)                     │
│  Files: FreshNoodle.UI/Services/ProductApiService.cs          │
│  Responsibilities:                                           │
│  - Make HTTP calls to API                                    │
│  - Serialize/deserialize JSON                                │
│  - Handle API errors                                         │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (Network - HTTPS)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│         LAYER 3: API Controller                              │
│  Files: FreshNoodle.API.Product/Controllers/                   │
│         ProductsController.cs                                │
│  Responsibilities:                                           │
│  - Receive HTTP requests                                     │
│  - Validate input                                            │
│  - Call service layer                                        │
│  - Return HTTP responses                                     │
│  - Handle authentication/authorization                       │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (Method Calls)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│         LAYER 4: Service Layer (Business Logic)              │
│  Files: FreshNoodle.API.Product/Services/ProductService.cs    │
│  Responsibilities:                                           │
│  - Implement business rules                                  │
│  - Convert between DTOs and Entities                         │
│  - Coordinate repository calls                               │
│  - Handle business validation                                │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (Method Calls)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│         LAYER 5: Repository Layer (Data Access)              │
│  Files: FreshNoodle.Infrastructure/Repositories/               │
│         Repository.cs                                        │
│  Responsibilities:                                           │
│  - CRUD operations on database                               │
│  - Execute EF Core queries                                   │
│  - Handle database transactions                              │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
                    (EF Core / SQL)
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│         LAYER 6: Database (SQL Server)                       │
│  Database: FreshNoodleDb                                       │
│  Tables: Products, Users                                     │
│  Responsibilities:                                           │
│  - Store data persistently                                   │
│  - Enforce data integrity                                    │
│  - Handle concurrent access                                  │
└─────────────────────────────────────────────────────────────┘
```

## Complete Workflow Example: Creating a Product

Let's walk through what happens when a user creates a new product.

### Step 1: User Interaction (UI Layer)

**File:** `FreshNoodle.UI/Pages/Products.razor`

```razor
<!-- User fills out the form -->
<EditForm Model="@productModel" OnValidSubmit="@HandleSubmit">
    <InputText @bind-Value="productModel.Name" />
    <InputNumber @bind-Value="productModel.Price" />
    <button type="submit">Create</button>
</EditForm>

@code {
    private ProductViewModel productModel = new();

    private async Task HandleSubmit()
    {
        // Call the API service
        var success = await ProductService.CreateProductAsync(productModel);
    }
}
```

**What happens:**
- User types "Laptop", "$999.99", "Gaming laptop"
- User clicks "Create" button
- Form validation runs
- `HandleSubmit()` method is called

### Step 2: API Service Call (UI Service Layer)

**File:** `FreshNoodle.UI/Services/ProductApiService.cs`

```csharp
public async Task<bool> CreateProductAsync(ProductViewModel product)
{
    try
    {
        // Send HTTP POST request to API
        var response = await _httpClient.PostAsJsonAsync("api/products", product);
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}
```

**What happens:**
- Converts C# object to JSON
- Sends HTTPS POST request to `https://localhost:7001/api/products`
- Includes JWT token in Authorization header
- Waits for response

**HTTP Request:**
```http
POST https://localhost:7001/api/products
Authorization: Bearer eyJhbGc...
Content-Type: application/json

{
  "name": "Laptop",
  "price": 999.99,
  "description": "Gaming laptop"
}
```

### Step 3: Controller Receives Request (API Controller Layer)

**File:** `FreshNoodle.API.Product/Controllers/ProductsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Requires authentication
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        // Validate input
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Call service layer
        var product = await _productService.CreateProductAsync(productDto);

        // Return HTTP 201 Created response
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }
}
```

**What happens:**
- ASP.NET Core deserializes JSON to `ProductDto`
- Checks JWT token (authentication)
- Validates model (required fields, data types)
- Calls service layer
- Returns HTTP response

### Step 4: Service Layer Processing (Business Logic Layer)

**File:** `FreshNoodle.API.Product/Services/ProductService.cs`

```csharp
public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        // Convert DTO to Entity (business object)
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Description = productDto.Description,
            CreatedDate = DateTime.UtcNow  // Business logic: set timestamp
        };

        // Call repository to save
        var createdProduct = await _productRepository.AddAsync(product);

        // Convert Entity back to DTO
        return new ProductDto
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
            Description = createdProduct.Description,
            CreatedDate = createdProduct.CreatedDate
        };
    }
}
```

**What happens:**
- Converts DTO (data transfer object) to Entity (database model)
- Applies business rules (sets CreatedDate)
- Calls repository to save data
- Converts result back to DTO
- Returns DTO to controller

**Why DTOs?** They protect our internal model from external changes and allow different representations for different layers.

### Step 5: Repository Saves to Database (Data Access Layer)

**File:** `FreshNoodle.Infrastructure/Repositories/Repository.cs`

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly FreshNoodleDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public virtual async Task<T> AddAsync(T entity)
    {
        // Add entity to EF Core change tracker
        await _dbSet.AddAsync(entity);

        // Save changes to database (executes SQL INSERT)
        await _context.SaveChangesAsync();

        // Entity now has ID from database
        return entity;
    }
}
```

**What happens:**
- EF Core generates SQL INSERT statement
- Executes SQL against database
- Database returns auto-generated ID
- Entity is updated with the ID

**SQL Generated:**
```sql
INSERT INTO Products (Name, Price, Description, CreatedDate)
VALUES (@p0, @p1, @p2, @p3);
SELECT Id FROM Products WHERE @@ROWCOUNT > 0 AND Id = scope_identity();
```

### Step 6: Database Storage

**Database:** SQL Server LocalDB
**Table:** Products

```sql
Id | Name    | Price  | Description    | CreatedDate
---|---------|--------|----------------|-------------------------
1  | Laptop  | 999.99 | Gaming laptop  | 2026-01-10 12:30:45.123
```

**What happens:**
- SQL Server validates data types
- Checks constraints (NOT NULL, data types)
- Assigns auto-incrementing ID
- Stores data in table
- Returns ID to EF Core

### Step 7: Response Flows Back Up

The response flows back up through each layer:

**Repository → Service:**
```csharp
Product { Id = 1, Name = "Laptop", ... }
```

**Service → Controller:**
```csharp
ProductDto { Id = 1, Name = "Laptop", ... }
```

**Controller → HTTP Response:**
```http
HTTP 201 Created
Location: /api/products/1
Content-Type: application/json

{
  "id": 1,
  "name": "Laptop",
  "price": 999.99,
  "description": "Gaming laptop",
  "createdDate": "2026-01-10T12:30:45.123Z"
}
```

**API Service → UI:**
```csharp
return true; // Success!
```

**UI → User:**
- Displays success message
- Refreshes product list
- New product appears on screen

## Key Concepts

### 1. Separation of Concerns

Each layer has ONE job:
- **UI**: Display and user interaction
- **API Service**: HTTP communication
- **Controller**: Route requests
- **Service**: Business logic
- **Repository**: Database operations
- **Database**: Data storage

### 2. Dependency Injection

Layers don't create their dependencies - they receive them:

```csharp
// BAD - tightly coupled
public class ProductService
{
    private Repository<Product> _repo = new Repository<Product>();
}

// GOOD - dependency injection
public class ProductService
{
    private readonly IRepository<Product> _repo;

    public ProductService(IRepository<Product> repo)
    {
        _repo = repo;  // Injected from Program.cs
    }
}
```

**Configured in:** `Program.cs`
```csharp
builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
builder.Services.AddScoped<IProductService, ProductService>();
```

### 3. DTOs vs Entities

**Entity** (Database Model):
```csharp
// FreshNoodle.Core/Entities/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**DTO** (Data Transfer Object):
```csharp
// FreshNoodle.Core/DTOs/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**Why both?**
- Entity = Internal representation (can change freely)
- DTO = External contract (stable API)
- Allows internal changes without breaking API

### 4. Async/Await Pattern

All data operations are asynchronous:

```csharp
public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
{
    var product = new Product { ... };
    var created = await _repository.AddAsync(product);  // Don't block
    return Map(created);
}
```

**Why?**
- Doesn't block threads while waiting for database
- Better performance and scalability
- Essential for web applications

## Complete CRUD Operations Flow

### CREATE (POST)
```
User fills form → UI calls API Service → HTTP POST → Controller
→ Service creates entity → Repository INSERT → Database stores
→ Returns ID → Service maps to DTO → Controller returns 201
→ API Service returns success → UI updates display
```

### READ (GET)
```
User opens page → UI calls API Service → HTTP GET → Controller
→ Service calls repository → Repository SELECT → Database returns data
→ Service maps to DTOs → Controller returns 200
→ API Service deserializes → UI displays list
```

### UPDATE (PUT)
```
User edits form → UI calls API Service → HTTP PUT → Controller
→ Service finds entity → Updates properties → Repository UPDATE
→ Database saves changes → Service maps to DTO → Controller returns 200
→ API Service returns success → UI updates display
```

### DELETE (DELETE)
```
User clicks delete → UI calls API Service → HTTP DELETE → Controller
→ Service calls repository → Repository DELETE → Database removes record
→ Returns success → Controller returns 200
→ API Service returns success → UI removes from list
```

## File Organization Summary

```
FreshNoodle/
├── FreshNoodle.Core/                    # Shared domain layer
│   ├── Entities/Product.cs           # Database model
│   ├── DTOs/ProductDto.cs            # API contract
│   └── Interfaces/IProductService.cs # Service interface
│
├── FreshNoodle.Infrastructure/          # Data access
│   └── Repositories/Repository.cs    # Generic CRUD implementation
│
├── FreshNoodle.API.Product/             # Product API
│   ├── Controllers/
│   │   └── ProductsController.cs     # HTTP endpoints
│   └── Services/
│       └── ProductService.cs         # Business logic
│
└── FreshNoodle.UI/                      # Blazor frontend
    ├── Pages/Products.razor          # UI page
    ├── Services/
    │   └── ProductApiService.cs      # HTTP client
    └── Models/ProductViewModel.cs    # UI model
```

## Authentication Flow

Authentication works similarly but with JWT tokens:

1. **Login:** User → API → Validate → Generate JWT → Return token
2. **Store:** UI stores token in localStorage
3. **Use:** Every API call includes token in Authorization header
4. **Validate:** API validates token on each request
5. **Authorize:** Controller checks [Authorize] attribute

## Best Practices

1. **Keep controllers thin** - Only routing and validation
2. **Business logic in services** - All rules and processing
3. **Repository only for data** - No business logic
4. **Use interfaces** - Enables dependency injection and testing
5. **Async everywhere** - All database operations
6. **DTOs for API** - Never expose entities directly
7. **Validate input** - At controller level
8. **Handle errors** - Try-catch in API services and services

## Common Patterns

### Pattern 1: Get All Items
```
UI → ProductApiService.GetAllProductsAsync()
  → HTTP GET /api/products
    → ProductsController.GetAllProducts()
      → ProductService.GetAllProductsAsync()
        → Repository.GetAllAsync()
          → Database SELECT * FROM Products
```

### Pattern 2: Get Single Item
```
UI → ProductApiService.GetProductByIdAsync(5)
  → HTTP GET /api/products/5
    → ProductsController.GetProductById(5)
      → ProductService.GetProductByIdAsync(5)
        → Repository.GetByIdAsync(5)
          → Database SELECT * FROM Products WHERE Id = 5
```

### Pattern 3: Create Item
```
UI → ProductApiService.CreateProductAsync(model)
  → HTTP POST /api/products + JSON body
    → ProductsController.CreateProduct(dto)
      → ProductService.CreateProductAsync(dto)
        → Repository.AddAsync(entity)
          → Database INSERT INTO Products
```

## Next Steps

Now that you understand the architecture, you can:
1. Follow the step-by-step guide to create a MoneyType feature
2. Apply the same pattern to create other features
3. Understand how to debug issues at each layer
4. Modify existing features with confidence

Remember: **Each layer has one job, and layers communicate through well-defined interfaces.**
