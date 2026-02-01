# FreshNoodle - Quick Start Guide

## What Was Built

A complete full-stack web application with:
- **Blazor WebAssembly** frontend
- **3 separate ASP.NET Core Web APIs** (Authentication, User Management, Products)
- **SQL Server** database with Entity Framework Core
- **JWT authentication** with BCrypt password hashing
- **Clean architecture** with proper separation of concerns

## Project Structure

```
FreshNoodle/
├── FreshNoodle.Core/              # Shared domain layer
│   ├── Entities/                # User, Product entities
│   ├── DTOs/                    # Data transfer objects
│   └── Interfaces/              # IRepository, IService interfaces
│
├── FreshNoodle.Infrastructure/    # Data access layer
│   ├── Data/                    # DbContext, DbInitializer
│   ├── Repositories/            # Repository implementations
│   └── Migrations/              # EF Core migrations
│
├── FreshNoodle.API.Authentication/ # Auth API
│   ├── Controllers/             # AuthController
│   ├── Services/                # AuthService
│   └── appsettings.json
│
├── FreshNoodle.API.User/          # User management API
│   ├── Controllers/             # UsersController
│   ├── Services/                # UserService
│   └── appsettings.json
│
├── FreshNoodle.API.Product/       # Product API
│   ├── Controllers/             # ProductsController
│   ├── Services/                # ProductService
│   └── appsettings.json
│
└── FreshNoodle.UI/                # Blazor WebAssembly frontend
    ├── Pages/                   # Login, Register, Products, Home
    ├── Components/              # RedirectToLogin
    ├── Services/                # AuthApiService, ProductApiService, AuthStateProvider
    ├── Models/                  # ViewModels
    └── wwwroot/
        └── appsettings.json     # API base URL configuration
```

## Running the Application

### Step 1: Open Two Command Prompts

**Command Prompt 1 - Authentication API:**
```bash
cd d:\Personal\FreshNoodle\FreshNoodle.API.Authentication
dotnet run
```

**Command Prompt 2 - Blazor UI:**
```bash
cd d:\Personal\FreshNoodle\FreshNoodle.UI
dotnet run
```

### Step 2: Note the URLs

After running, you'll see output like:
```
Authentication API: https://localhost:7001
Blazor UI: https://localhost:5001
```

### Step 3: Configure UI to Point to API

Edit [FreshNoodle.UI/wwwroot/appsettings.json](FreshNoodle.UI/wwwroot/appsettings.json) and set:
```json
{
  "ApiBaseAddress": "https://localhost:7001"
}
```
(Replace 7001 with your actual Authentication API port)

### Step 4: Access the Application

Open your browser and navigate to: `https://localhost:5001` (or your UI port)

## Sample Login Credentials

The database is automatically seeded with sample data:

| Username | Password | Email |
|----------|----------|-------|
| admin | admin123 | admin@FreshNoodle.com |
| testuser | test123 | test@FreshNoodle.com |

## Features Implemented

### Authentication
- User registration with validation
- User login with JWT tokens
- Password hashing with BCrypt
- Token-based authentication
- Logout functionality

### Product Management (Requires Authentication)
- View all products
- Create new products
- Edit existing products
- Delete products
- Form validation

### Security
- Protected API endpoints (require authentication)
- Protected pages (redirect to login)
- JWT token storage in browser localStorage
- CORS configuration for cross-origin requests

## API Endpoints

### Authentication API (https://localhost:7001)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/logout` - Logout user

### Product API (https://localhost:7003 or 7002)
- `GET /api/products` - Get all products (auth required)
- `GET /api/products/{id}` - Get product by ID (auth required)
- `POST /api/products` - Create product (auth required)
- `PUT /api/products/{id}` - Update product (auth required)
- `DELETE /api/products/{id}` - Delete product (auth required)

### User API (https://localhost:7002 or 7003)
- `GET /api/users` - Get all users (auth required)
- `GET /api/users/{id}` - Get user by ID (auth required)
- `POST /api/users` - Create user (auth required)
- `PUT /api/users/{id}` - Update user (auth required)
- `DELETE /api/users/{id}` - Delete user (auth required)

## Database

The application uses **SQL Server LocalDB** with the database name `FreshNoodleDb`.

Connection string (in all API appsettings.json):
```
Server=(localdb)\mssqllocaldb;Database=FreshNoodleDb;Trusted_Connection=true;TrustServerCertificate=true;
```

The database is automatically:
- Created on first run
- Migrated to the latest schema
- Seeded with sample users and products

## Testing the Application

1. **Register a new account**
   - Navigate to Register page
   - Fill in username, email, password
   - Click Register
   - You'll be automatically logged in

2. **Login with sample account**
   - Navigate to Login page
   - Username: `admin`, Password: `admin123`
   - Click Login

3. **Manage products**
   - After login, go to Products page
   - View the 4 sample products
   - Click "Add New Product" to create one
   - Edit or delete existing products

4. **Logout**
   - Click Logout in the navigation menu
   - You'll be redirected to home page
   - Protected pages will redirect to login

## Architecture Highlights

### Clean Architecture Layers
1. **Core** - Domain entities and interfaces (no dependencies)
2. **Infrastructure** - Data access (depends on Core)
3. **API** - Application/presentation layer (depends on Core & Infrastructure)
4. **UI** - Blazor presentation layer (depends on nothing, calls APIs)

### Design Patterns
- Repository Pattern for data access
- Dependency Injection throughout
- JWT bearer token authentication
- DTO pattern for API contracts
- Service layer for business logic

## Troubleshooting

### "Cannot connect to database"
- Ensure SQL Server LocalDB is installed
- Check connection string in API appsettings.json

### "CORS error in browser"
- Ensure API is running before starting UI
- Check CORS configuration in API Program.cs matches UI URL

### "401 Unauthorized"
- Ensure you're logged in
- Check that JWT token is being sent in requests
- Verify API JWT settings match between all APIs

### "API not found"
- Check ApiBaseAddress in UI wwwroot/appsettings.json
- Ensure Authentication API is running
- Verify the port number matches

## Next Steps

- Explore the code structure
- Add new features (user profile, categories, etc.)
- Implement additional APIs
- Deploy to Azure/AWS
- Add unit tests

For detailed documentation, see [README.md](README.md).
