# Step-by-Step Guide: Creating MoneyType Feature

This guide will walk you through creating a complete MoneyType feature from scratch, following the same workflow as Products.

## What We're Building

**MoneyType** will represent different types of money transactions (e.g., Income, Expense, Transfer, Investment).

### Fields:
- **Id** (int) - Auto-generated
- **Name** (string) - Type name (e.g., "Income", "Expense")
- **Description** (string) - Description of the type
- **IsActive** (bool) - Whether this type is currently in use
- **CreatedDate** (DateTime) - When it was created

## Prerequisites

Make sure you've read [ARCHITECTURE_GUIDE.md](ARCHITECTURE_GUIDE.md) to understand the workflow.

---

## Phase 1: Create the Domain Model (Core Layer)

### Step 1.1: Create the Entity

**File to create:** `FreshNoodle.Core/Entities/MoneyType.cs`

```csharp
namespace FreshNoodle.Core.Entities;

public class MoneyType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

**Why?** This represents the database table structure.

### Step 1.2: Create the DTO

**File to create:** `FreshNoodle.Core/DTOs/MoneyTypeDto.cs`

```csharp
namespace FreshNoodle.Core.DTOs;

public class MoneyTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**Why?** This is the object that travels between API and UI.

### Step 1.3: Create the Service Interface

**File to create:** `FreshNoodle.Core/Interfaces/IMoneyTypeService.cs`

```csharp
using FreshNoodle.Core.DTOs;

namespace FreshNoodle.Core.Interfaces;

public interface IMoneyTypeService
{
    Task<MoneyTypeDto?> GetMoneyTypeByIdAsync(int id);
    Task<IEnumerable<MoneyTypeDto>> GetAllMoneyTypesAsync();
    Task<MoneyTypeDto> CreateMoneyTypeAsync(MoneyTypeDto moneyTypeDto);
    Task<MoneyTypeDto> UpdateMoneyTypeAsync(int id, MoneyTypeDto moneyTypeDto);
    Task<bool> DeleteMoneyTypeAsync(int id);
}
```

**Why?** Defines what operations our service can perform.

---

## Phase 2: Update Database (Infrastructure Layer)

### Step 2.1: Add DbSet to DbContext

**File to edit:** `FreshNoodle.Infrastructure/Data/FreshNoodleDbContext.cs`

Add this property inside the class:

```csharp
public DbSet<MoneyType> MoneyTypes { get; set; }
```

And add configuration in `OnModelCreating` method:

```csharp
// MoneyType configuration
modelBuilder.Entity<MoneyType>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
    entity.Property(e => e.Description).HasMaxLength(500);
    entity.Property(e => e.IsActive).IsRequired();
    entity.HasIndex(e => e.Name).IsUnique();
});
```

**Complete file should look like:**

```csharp
using Microsoft.EntityFrameworkCore;
using FreshNoodle.Core.Entities;

namespace FreshNoodle.Infrastructure.Data;

public class FreshNoodleDbContext : DbContext
{
    public FreshNoodleDbContext(DbContextOptions<FreshNoodleDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<MoneyType> MoneyTypes { get; set; }  // NEW

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // MoneyType configuration - NEW
        modelBuilder.Entity<MoneyType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}
```

**Why?** Tells EF Core how to map MoneyType to database.

### Step 2.2: Create Database Migration

Open a terminal and run:

```bash
cd FreshNoodle.Infrastructure
dotnet ef migrations add AddMoneyType --startup-project ../FreshNoodle.API.Authentication
```

**What this does:** Creates migration files to add MoneyTypes table.

### Step 2.3: Update Seed Data (Optional)

**File to edit:** `FreshNoodle.Infrastructure/Data/DbInitializer.cs`

Add this code after seeding products:

```csharp
// Seed sample money types
var moneyTypes = new[]
{
    new MoneyType
    {
        Name = "Income",
        Description = "Money received",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    },
    new MoneyType
    {
        Name = "Expense",
        Description = "Money spent",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    },
    new MoneyType
    {
        Name = "Transfer",
        Description = "Money transferred between accounts",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    },
    new MoneyType
    {
        Name = "Investment",
        Description = "Money invested",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    }
};

await context.MoneyTypes.AddRangeAsync(moneyTypes);
await context.SaveChangesAsync();
```

**Complete updated section:**

```csharp
public static async Task SeedDataAsync(FreshNoodleDbContext context)
{
    await context.Database.EnsureCreatedAsync();

    if (context.Users.Any() || context.Products.Any() || context.MoneyTypes.Any())
    {
        return; // Database has been seeded
    }

    // ... existing user seeding code ...

    // ... existing product seeding code ...

    // Seed sample money types - NEW
    var moneyTypes = new[]
    {
        new MoneyType
        {
            Name = "Income",
            Description = "Money received",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        },
        new MoneyType
        {
            Name = "Expense",
            Description = "Money spent",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        },
        new MoneyType
        {
            Name = "Transfer",
            Description = "Money transferred between accounts",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        },
        new MoneyType
        {
            Name = "Investment",
            Description = "Money invested",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        }
    };

    await context.MoneyTypes.AddRangeAsync(moneyTypes);
    await context.SaveChangesAsync();
}
```

---

## Phase 3: Create the API Project

### Step 3.1: Create New API Project

Open terminal:

```bash
cd d:\Personal\FreshNoodle
dotnet new webapi -n FreshNoodle.API.MoneyType -f net9.0
dotnet sln FreshNoodle.sln add FreshNoodle.API.MoneyType/FreshNoodle.API.MoneyType.csproj
```

### Step 3.2: Add Project References

```bash
dotnet add FreshNoodle.API.MoneyType/FreshNoodle.API.MoneyType.csproj reference FreshNoodle.Core/FreshNoodle.Core.csproj
dotnet add FreshNoodle.API.MoneyType/FreshNoodle.API.MoneyType.csproj reference FreshNoodle.Infrastructure/FreshNoodle.Infrastructure.csproj
```

### Step 3.3: Add Required Packages

```bash
dotnet add FreshNoodle.API.MoneyType/FreshNoodle.API.MoneyType.csproj package Microsoft.AspNetCore.Authentication.JwtBearer -v 9.0.0
dotnet add FreshNoodle.API.MoneyType/FreshNoodle.API.MoneyType.csproj package Microsoft.EntityFrameworkCore.Design -v 9.0.0
```

### Step 3.4: Delete Template Files

```bash
del FreshNoodle.API.MoneyType\Controllers\WeatherForecastController.cs
del FreshNoodle.API.MoneyType\WeatherForecast.cs
```

### Step 3.5: Create Service Implementation

**File to create:** `FreshNoodle.API.MoneyType/Services/MoneyTypeService.cs`

```csharp
using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.API.MoneyType.Services;

public class MoneyTypeService : IMoneyTypeService
{
    private readonly IRepository<Core.Entities.MoneyType> _moneyTypeRepository;

    public MoneyTypeService(IRepository<Core.Entities.MoneyType> moneyTypeRepository)
    {
        _moneyTypeRepository = moneyTypeRepository;
    }

    public async Task<MoneyTypeDto?> GetMoneyTypeByIdAsync(int id)
    {
        var moneyType = await _moneyTypeRepository.GetByIdAsync(id);
        if (moneyType == null)
            return null;

        return new MoneyTypeDto
        {
            Id = moneyType.Id,
            Name = moneyType.Name,
            Description = moneyType.Description,
            IsActive = moneyType.IsActive,
            CreatedDate = moneyType.CreatedDate
        };
    }

    public async Task<IEnumerable<MoneyTypeDto>> GetAllMoneyTypesAsync()
    {
        var moneyTypes = await _moneyTypeRepository.GetAllAsync();
        return moneyTypes.Select(mt => new MoneyTypeDto
        {
            Id = mt.Id,
            Name = mt.Name,
            Description = mt.Description,
            IsActive = mt.IsActive,
            CreatedDate = mt.CreatedDate
        });
    }

    public async Task<MoneyTypeDto> CreateMoneyTypeAsync(MoneyTypeDto moneyTypeDto)
    {
        var moneyType = new Core.Entities.MoneyType
        {
            Name = moneyTypeDto.Name,
            Description = moneyTypeDto.Description,
            IsActive = moneyTypeDto.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var createdMoneyType = await _moneyTypeRepository.AddAsync(moneyType);

        return new MoneyTypeDto
        {
            Id = createdMoneyType.Id,
            Name = createdMoneyType.Name,
            Description = createdMoneyType.Description,
            IsActive = createdMoneyType.IsActive,
            CreatedDate = createdMoneyType.CreatedDate
        };
    }

    public async Task<MoneyTypeDto> UpdateMoneyTypeAsync(int id, MoneyTypeDto moneyTypeDto)
    {
        var moneyType = await _moneyTypeRepository.GetByIdAsync(id);
        if (moneyType == null)
            throw new KeyNotFoundException($"MoneyType with ID {id} not found");

        moneyType.Name = moneyTypeDto.Name;
        moneyType.Description = moneyTypeDto.Description;
        moneyType.IsActive = moneyTypeDto.IsActive;

        var updatedMoneyType = await _moneyTypeRepository.UpdateAsync(moneyType);

        return new MoneyTypeDto
        {
            Id = updatedMoneyType.Id,
            Name = updatedMoneyType.Name,
            Description = updatedMoneyType.Description,
            IsActive = updatedMoneyType.IsActive,
            CreatedDate = updatedMoneyType.CreatedDate
        };
    }

    public async Task<bool> DeleteMoneyTypeAsync(int id)
    {
        return await _moneyTypeRepository.DeleteAsync(id);
    }
}
```

**Why?** Implements business logic for MoneyType operations.

### Step 3.6: Create Controller

**File to create:** `FreshNoodle.API.MoneyType/Controllers/MoneyTypesController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.API.MoneyType.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoneyTypesController : ControllerBase
{
    private readonly IMoneyTypeService _moneyTypeService;

    public MoneyTypesController(IMoneyTypeService moneyTypeService)
    {
        _moneyTypeService = moneyTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMoneyTypes()
    {
        var moneyTypes = await _moneyTypeService.GetAllMoneyTypesAsync();
        return Ok(moneyTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMoneyTypeById(int id)
    {
        var moneyType = await _moneyTypeService.GetMoneyTypeByIdAsync(id);
        if (moneyType == null)
            return NotFound(new { message = $"MoneyType with ID {id} not found" });

        return Ok(moneyType);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMoneyType([FromBody] MoneyTypeDto moneyTypeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var moneyType = await _moneyTypeService.CreateMoneyTypeAsync(moneyTypeDto);
        return CreatedAtAction(nameof(GetMoneyTypeById), new { id = moneyType.Id }, moneyType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMoneyType(int id, [FromBody] MoneyTypeDto moneyTypeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedMoneyType = await _moneyTypeService.UpdateMoneyTypeAsync(id, moneyTypeDto);
            return Ok(updatedMoneyType);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMoneyType(int id)
    {
        var result = await _moneyTypeService.DeleteMoneyTypeAsync(id);
        if (!result)
            return NotFound(new { message = $"MoneyType with ID {id} not found" });

        return Ok(new { message = "MoneyType deleted successfully" });
    }
}
```

**Why?** Handles HTTP requests for MoneyType operations.

### Step 3.7: Configure Program.cs

**File to edit:** `FreshNoodle.API.MoneyType/Program.cs`

Replace entire content with:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FreshNoodle.API.MoneyType.Services;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using FreshNoodle.Infrastructure.Data;
using FreshNoodle.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure database
builder.Services.AddDbContext<FreshNoodleDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FreshNoodle.Infrastructure")));

// Register repositories and services
builder.Services.AddScoped<IRepository<Core.Entities.MoneyType>, Repository<Core.Entities.MoneyType>>();
builder.Services.AddScoped<IMoneyTypeService, MoneyTypeService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForFreshNoodleApplication12345";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "FreshNoodleAPI",
        ValidAudience = jwtSettings["Audience"] ?? "FreshNoodleClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:5001",
                "http://localhost:5000",
                "https://localhost:5173",
                "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 3.8: Configure appsettings.json

**File to edit:** `FreshNoodle.API.MoneyType/appsettings.json`

Replace with:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FreshNoodleDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForFreshNoodleApplication12345!@#",
    "Issuer": "FreshNoodleAPI",
    "Audience": "FreshNoodleClient"
  }
}
```

### Step 3.9: Configure Launch Settings

**File to edit:** `FreshNoodle.API.MoneyType/Properties/launchSettings.json`

Replace with:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7004;http://localhost:5104",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5104",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## Phase 4: Create UI Components

### Step 4.1: Create View Model

**File to create:** `FreshNoodle.UI/Models/MoneyTypeViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace FreshNoodle.UI.Models;

public class MoneyTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }
}
```

### Step 4.2: Create API Service

**File to create:** `FreshNoodle.UI/Services/MoneyTypeApiService.cs`

```csharp
using System.Net.Http.Json;
using FreshNoodle.UI.Models;

namespace FreshNoodle.UI.Services;

public class MoneyTypeApiService
{
    private readonly HttpClient _httpClient;

    public MoneyTypeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MoneyTypeViewModel>> GetAllMoneyTypesAsync()
    {
        try
        {
            var moneyTypes = await _httpClient.GetFromJsonAsync<List<MoneyTypeViewModel>>("api/moneytypes");
            return moneyTypes ?? new List<MoneyTypeViewModel>();
        }
        catch
        {
            return new List<MoneyTypeViewModel>();
        }
    }

    public async Task<MoneyTypeViewModel?> GetMoneyTypeByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MoneyTypeViewModel>($"api/moneytypes/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateMoneyTypeAsync(MoneyTypeViewModel moneyType)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/moneytypes", moneyType);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateMoneyTypeAsync(int id, MoneyTypeViewModel moneyType)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/moneytypes/{id}", moneyType);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteMoneyTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/moneytypes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

### Step 4.3: Register Service in Program.cs

**File to edit:** `FreshNoodle.UI/Program.cs`

Add this line after the existing service registrations:

```csharp
// Register API services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<MoneyTypeApiService>();  // NEW
```

**Complete section should look like:**

```csharp
// Register API services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<MoneyTypeApiService>();
```

### Step 4.4: Create MoneyTypes Page

**File to create:** `FreshNoodle.UI/Pages/MoneyTypes.razor`

```razor
@page "/moneytypes"
@using FreshNoodle.UI.Models
@using FreshNoodle.UI.Services
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]
@inject MoneyTypeApiService MoneyTypeService
@inject NavigationManager Navigation

<PageTitle>Money Types - FreshNoodle</PageTitle>

<div class="container mt-5">
    <div class="row">
        <div class="col-md-12">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h2>Money Types</h2>
                <button class="btn btn-success" @onclick="ShowCreateForm">Add New Money Type</button>
            </div>

            @if (!string.IsNullOrEmpty(successMessage))
            {
                <div class="alert alert-success">@successMessage</div>
            }

            @if (!string.IsNullOrEmpty(errorMessage))
            {
                <div class="alert alert-danger">@errorMessage</div>
            }

            @if (showForm)
            {
                <div class="card mb-3">
                    <div class="card-header">
                        <h4>@(editingMoneyType != null ? "Edit Money Type" : "New Money Type")</h4>
                    </div>
                    <div class="card-body">
                        <EditForm Model="@moneyTypeModel" OnValidSubmit="@HandleSubmit">
                            <DataAnnotationsValidator />
                            <ValidationSummary />

                            <div class="mb-3">
                                <label for="name" class="form-label">Name</label>
                                <InputText id="name" class="form-control" @bind-Value="moneyTypeModel.Name" />
                                <ValidationMessage For="@(() => moneyTypeModel.Name)" />
                            </div>

                            <div class="mb-3">
                                <label for="description" class="form-label">Description</label>
                                <InputTextArea id="description" class="form-control" rows="3" @bind-Value="moneyTypeModel.Description" />
                                <ValidationMessage For="@(() => moneyTypeModel.Description)" />
                            </div>

                            <div class="mb-3 form-check">
                                <InputCheckbox id="isActive" class="form-check-input" @bind-Value="moneyTypeModel.IsActive" />
                                <label for="isActive" class="form-check-label">Active</label>
                            </div>

                            <button type="submit" class="btn btn-primary" disabled="@isLoading">
                                @if (isLoading)
                                {
                                    <span>Saving...</span>
                                }
                                else
                                {
                                    <span>@(editingMoneyType != null ? "Update" : "Create")</span>
                                }
                            </button>
                            <button type="button" class="btn btn-secondary" @onclick="CancelForm">Cancel</button>
                        </EditForm>
                    </div>
                </div>
            }

            @if (moneyTypes == null)
            {
                <p>Loading money types...</p>
            }
            else if (moneyTypes.Count == 0)
            {
                <p>No money types found. Create your first money type above.</p>
            }
            else
            {
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Status</th>
                            <th>Created Date</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var moneyType in moneyTypes)
                        {
                            <tr>
                                <td>@moneyType.Name</td>
                                <td>@moneyType.Description</td>
                                <td>
                                    @if (moneyType.IsActive)
                                    {
                                        <span class="badge bg-success">Active</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-secondary">Inactive</span>
                                    }
                                </td>
                                <td>@moneyType.CreatedDate.ToShortDateString()</td>
                                <td>
                                    <button class="btn btn-sm btn-primary" @onclick="() => EditMoneyType(moneyType)">Edit</button>
                                    <button class="btn btn-sm btn-danger" @onclick="() => DeleteMoneyType(moneyType.Id)">Delete</button>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            }
        </div>
    </div>
</div>

@code {
    private List<MoneyTypeViewModel>? moneyTypes;
    private MoneyTypeViewModel moneyTypeModel = new();
    private MoneyTypeViewModel? editingMoneyType = null;
    private bool showForm = false;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    private bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadMoneyTypes();
    }

    private async Task LoadMoneyTypes()
    {
        moneyTypes = await MoneyTypeService.GetAllMoneyTypesAsync();
    }

    private void ShowCreateForm()
    {
        moneyTypeModel = new MoneyTypeViewModel { IsActive = true };
        editingMoneyType = null;
        showForm = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;
    }

    private void EditMoneyType(MoneyTypeViewModel moneyType)
    {
        moneyTypeModel = new MoneyTypeViewModel
        {
            Id = moneyType.Id,
            Name = moneyType.Name,
            Description = moneyType.Description,
            IsActive = moneyType.IsActive
        };
        editingMoneyType = moneyType;
        showForm = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;
    }

    private void CancelForm()
    {
        showForm = false;
        moneyTypeModel = new();
        editingMoneyType = null;
    }

    private async Task HandleSubmit()
    {
        isLoading = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;

        bool success;

        if (editingMoneyType != null)
        {
            success = await MoneyTypeService.UpdateMoneyTypeAsync(moneyTypeModel.Id, moneyTypeModel);
            successMessage = success ? "Money type updated successfully!" : "";
        }
        else
        {
            success = await MoneyTypeService.CreateMoneyTypeAsync(moneyTypeModel);
            successMessage = success ? "Money type created successfully!" : "";
        }

        if (success)
        {
            await LoadMoneyTypes();
            CancelForm();
        }
        else
        {
            errorMessage = "Failed to save money type. Please try again.";
        }

        isLoading = false;
    }

    private async Task DeleteMoneyType(int id)
    {
        var success = await MoneyTypeService.DeleteMoneyTypeAsync(id);

        if (success)
        {
            successMessage = "Money type deleted successfully!";
            await LoadMoneyTypes();
        }
        else
        {
            errorMessage = "Failed to delete money type.";
        }
    }
}
```

### Step 4.5: Update Navigation Menu

**File to edit:** `FreshNoodle.UI/Layout/NavMenu.razor`

Add this inside the `<Authorized>` section:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="moneytypes">
        <span class="bi bi-cash-stack" aria-hidden="true"></span> Money Types
    </NavLink>
</div>
```

**Complete Authorized section should look like:**

```razor
<Authorized>
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="products">
            <span class="bi bi-box-seam-fill" aria-hidden="true"></span> Products
        </NavLink>
    </div>
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="moneytypes">
            <span class="bi bi-cash-stack" aria-hidden="true"></span> Money Types
        </NavLink>
    </div>
    <div class="nav-item px-3">
        <a class="nav-link" href="#" @onclick="HandleLogout" @onclick:preventDefault>
            <span class="bi bi-box-arrow-right" aria-hidden="true"></span> Logout
        </a>
    </div>
</Authorized>
```

---

## Phase 5: Test Your Implementation

### Step 5.1: Build the Solution

```bash
cd d:\Personal\FreshNoodle
dotnet build FreshNoodle.sln
```

Make sure there are no errors.

### Step 5.2: Apply Database Migration

Start the Authentication API (it will auto-apply migrations):

```bash
cd FreshNoodle.API.Authentication
dotnet run
```

Wait for: `Now listening on: https://localhost:7001`

The migration should run automatically and seed the data.

### Step 5.3: Test the MoneyType API (Optional)

In a new terminal:

```bash
# First, login to get a token
curl -k -X POST https://localhost:7001/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"admin123\"}"

# Copy the token from the response, then test MoneyType API
# Replace YOUR_TOKEN with the actual token
curl -k -X GET https://localhost:7001/api/moneytypes -H "Authorization: Bearer YOUR_TOKEN"
```

### Step 5.4: Start the UI and Test

In another terminal:

```bash
cd FreshNoodle.UI
dotnet run
```

Open browser to the URL shown, login, and:

1. Click "Money Types" in navigation
2. You should see 4 seeded money types
3. Try creating a new one
4. Try editing one
5. Try deleting one

---

## Verification Checklist

- [ ] MoneyType entity created in Core
- [ ] MoneyTypeDto created in Core
- [ ] IMoneyTypeService interface created
- [ ] DbSet added to DbContext
- [ ] Migration created and applied
- [ ] API project created and configured
- [ ] MoneyTypeService implemented
- [ ] MoneyTypesController created
- [ ] MoneyTypeViewModel created in UI
- [ ] MoneyTypeApiService created in UI
- [ ] MoneyTypes.razor page created
- [ ] Navigation menu updated
- [ ] Solution builds successfully
- [ ] Can view money types in UI
- [ ] Can create new money types
- [ ] Can edit money types
- [ ] Can delete money types

---

## Common Issues and Solutions

### Issue: Build errors about MoneyType not found

**Solution:** Make sure you added `using FreshNoodle.Core.Entities;` at the top of files.

### Issue: Migration doesn't create table

**Solution:** Delete the old database and restart the API:
```bash
sqllocaldb stop mssqllocaldb
sqllocaldb delete mssqllocaldb
sqllocaldb create mssqllocaldb
```

### Issue: Navigation menu doesn't show Money Types

**Solution:** Make sure you're logged in and added the NavLink inside the `<Authorized>` section.

### Issue: API returns 404

**Solution:** Make sure the Authentication API is running, not the MoneyType API. The MoneyType endpoints are accessed through the Auth API if using the same database.

**Alternative:** Run the MoneyType API separately on port 7004 and update the UI to call that endpoint.

### Issue: Can't see data in UI

**Solution:**
1. Check browser console (F12) for errors
2. Make sure you're logged in
3. Verify the API is running
4. Check the network tab to see if API calls are being made

---

## Next Steps

Congratulations! You've successfully created a complete MoneyType feature following the clean architecture pattern.

**What you learned:**
- Creating entities and DTOs
- Updating database schema with migrations
- Implementing service layer logic
- Creating API controllers
- Building Blazor UI components
- Connecting all layers together

**Try creating another feature on your own:**
- Account (bank accounts, wallets)
- Transaction (money movements)
- Category (transaction categories)
- Budget (spending limits)

Follow the same steps - the pattern is always the same!

---

## Quick Command Reference

```bash
# Create new API project
dotnet new webapi -n FreshNoodle.API.YourFeature -f net9.0
dotnet sln add FreshNoodle.API.YourFeature/FreshNoodle.API.YourFeature.csproj

# Add references
dotnet add FreshNoodle.API.YourFeature reference FreshNoodle.Core FreshNoodle.Infrastructure

# Create migration
cd FreshNoodle.Infrastructure
dotnet ef migrations add AddYourFeature --startup-project ../FreshNoodle.API.Authentication

# Build solution
dotnet build FreshNoodle.sln

# Run API
cd FreshNoodle.API.Authentication
dotnet run

# Run UI
cd FreshNoodle.UI
dotnet run
```
