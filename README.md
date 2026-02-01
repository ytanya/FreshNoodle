# FreshNoodle

A full-stack web application built with **Blazor WebAssembly**, **ASP.NET Core Web API**, and **SQL Server** for product management with authentication.

## Project Overview

FreshNoodle is a scalable web application that demonstrates:
- User authentication (Login/Register) with JWT
- Product CRUD operations
- Clean layered architecture
- RESTful API design
- Blazor WebAssembly frontend
- Entity Framework Core with SQL Server

## Technology Stack

### Frontend
- **Blazor WebAssembly** (.NET 9)
- Razor components
- HTTP API communication
- JWT-based authentication

### Backend
- **ASP.NET Core Web API** (.NET 9)
- RESTful controllers
- JWT authentication with BCrypt password hashing
- Dependency Injection
- Layered architecture (Controller → Service → Repository)

### Database
- **SQL Server** (LocalDB for development)
- **Entity Framework Core 9.0** (Code First)

## Solution Structure

```
FreshNoodle.sln
│
├── FreshNoodle.UI                    # Blazor WebAssembly frontend
├── FreshNoodle.API.Authentication    # Authentication API (Login/Register)
├── FreshNoodle.API.User              # User management API
├── FreshNoodle.API.Product           # Product CRUD API
├── FreshNoodle.Core                  # Shared entities, DTOs, and interfaces
└── FreshNoodle.Infrastructure        # Data access layer (DbContext, Repositories)
```

## Prerequisites

Before running the application, ensure you have:

1. **.NET 9.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download

2. **SQL Server LocalDB** or SQL Server
   - Included with Visual Studio or download SQL Server Express

3. **Visual Studio 2022** (recommended) or Visual Studio Code

## Getting Started

### 1. Clone or Extract the Project

Navigate to the project directory:
```bash
cd d:\Personal\FreshNoodle
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Update Database Connection String (Optional)

The default connection string uses SQL Server LocalDB. If you want to use a different SQL Server instance, update the `ConnectionStrings` in:

- `FreshNoodle.API.Authentication/appsettings.json`
- `FreshNoodle.API.User/appsettings.json`
- `FreshNoodle.API.Product/appsettings.json`

Default connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FreshNoodleDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### 4. Apply Database Migrations

The database will be automatically created and seeded when you run the Authentication API for the first time. The migration runs automatically on startup.

Alternatively, you can manually run migrations:

```bash
cd FreshNoodle.Infrastructure
dotnet ef database update --startup-project ../FreshNoodle.API.Authentication
```

### 5. Run the Application

You need to run **both the API and the UI** simultaneously.

#### Option 1: Using Multiple Terminals

**Terminal 1 - Run Authentication API:**
```bash
cd FreshNoodle.API.Authentication
dotnet run
```
Note the HTTPS URL (e.g., `https://localhost:7001`)

**Terminal 2 - Run Blazor UI:**
```bash
cd FreshNoodle.UI
dotnet run
```

#### Option 2: Using Visual Studio

1. Right-click on the solution in Solution Explorer
2. Select "Configure Startup Projects"
3. Choose "Multiple startup projects"
4. Set `FreshNoodle.API.Authentication` and `FreshNoodle.UI` to **Start**
5. Click OK and press F5

### 6. Configure API Base Address

Update the API base address in `FreshNoodle.UI/wwwroot/appsettings.json` to match your Authentication API URL:

```json
{
  "ApiBaseAddress": "https://localhost:7001"
}
```

Replace `7001` with the actual port number from step 5.

## Sample Data

The application automatically seeds the following data on first run:

### Sample Users
| Username | Password | Email |
|----------|----------|-------|
| admin | admin123 | admin@FreshNoodle.com |
| testuser | test123 | test@FreshNoodle.com |

### Sample Products
- Laptop - $999.99
- Mouse - $29.99
- Keyboard - $79.99
- Monitor - $299.99

## Usage

### 1. Register a New Account
1. Navigate to the Register page
2. Enter username, email, and password
3. Click "Register"
4. You'll be automatically logged in

### 2. Login
1. Navigate to the Login page
2. Enter credentials (use sample users or your registered account)
3. Click "Login"

### 3. Manage Products
1. After login, navigate to "Products"
2. View all products
3. Add new products using "Add New Product" button
4. Edit or delete existing products

## API Endpoints

### Authentication API (Port 7001)
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout

### User API (Port 7002)
- `GET /api/users` - Get all users (requires auth)
- `GET /api/users/{id}` - Get user by ID (requires auth)
- `POST /api/users` - Create user (requires auth)
- `PUT /api/users/{id}` - Update user (requires auth)
- `DELETE /api/users/{id}` - Delete user (requires auth)

### Product API (Port 7003)
- `GET /api/products` - Get all products (requires auth)
- `GET /api/products/{id}` - Get product by ID (requires auth)
- `POST /api/products` - Create product (requires auth)
- `PUT /api/products/{id}` - Update product (requires auth)
- `DELETE /api/products/{id}` - Delete product (requires auth)

## Architecture

### Clean Architecture Layers

1. **FreshNoodle.Core** - Domain layer
   - Entities (User, Product)
   - DTOs (Data Transfer Objects)
   - Interfaces (IRepository, IService)

2. **FreshNoodle.Infrastructure** - Data access layer
   - DbContext
   - Repository implementations
   - Database migrations

3. **FreshNoodle.API.*** - Application layer
   - Controllers
   - Services (business logic)
   - Authentication/Authorization

4. **FreshNoodle.UI** - Presentation layer
   - Blazor components
   - Pages
   - API service clients

## Security Features

- **Password Hashing** - BCrypt.Net for secure password storage
- **JWT Authentication** - Stateless authentication with JSON Web Tokens
- **CORS** - Configured for secure cross-origin requests
- **Authorization** - Protected API endpoints and pages

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server LocalDB is installed
- Check connection string in `appsettings.json`
- Try running migrations manually

### CORS Errors
- Ensure API CORS configuration matches UI URL
- Check that both API and UI are running

### Port Conflicts
- Change ports in `Properties/launchSettings.json` if needed
- Update `ApiBaseAddress` in UI configuration

## AWS Deployment (Future)

The application is designed for AWS deployment:
- **UI** → S3 + CloudFront (static hosting)
- **API** → EC2 / ECS (containerized)
- **Database** → RDS SQL Server

## Contributing

This is a demonstration project following clean architecture principles and best practices for ASP.NET Core and Blazor development.

## License

This project is for educational purposes.

## Contact

For questions or issues, please refer to the project documentation.
