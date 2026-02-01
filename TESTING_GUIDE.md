# FreshNoodle - Testing Guide

## Quick Start (For Testing Login Issues)

### Step 1: Start the Authentication API

Open a command prompt and run:
```cmd
cd d:\Personal\FreshNoodle
start-api.cmd
```

OR manually:
```cmd
cd d:\Personal\FreshNoodle\FreshNoodle.API.Authentication
dotnet run
```

You should see:
```
Now listening on: https://localhost:7001
Now listening on: http://localhost:5001
```

### Step 2: Test the API Directly

Open a second command prompt and test the login endpoint:

**Test with admin user:**
```bash
curl -k -X POST https://localhost:7001/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"admin123\"}"
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGc...(long JWT token)...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@FreshNoodle.com",
    "createdDate": "2026-01-10T..."
  }
}
```

**Test with testuser:**
```bash
curl -k -X POST https://localhost:7001/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"testuser\",\"password\":\"test123\"}"
```

### Step 3: Configure and Start the Blazor UI

1. **Verify API URL in UI configuration:**

   Open `d:\Personal\FreshNoodle\FreshNoodle.UI\wwwroot\appsettings.json`

   Should contain:
   ```json
   {
     "ApiBaseAddress": "https://localhost:7001"
   }
   ```

2. **Start the UI:**
   ```cmd
   cd d:\Personal\FreshNoodle
   start-ui.cmd
   ```

   OR manually:
   ```cmd
   cd d:\Personal\FreshNoodle\FreshNoodle.UI
   dotnet run
   ```

3. **Open the browser:**

   The UI will output something like:
   ```
   Now listening on: https://localhost:5173
   ```

   Open your browser to that URL (e.g., `https://localhost:5173`)

### Step 4: Test Login in Browser

1. Click "Login" in the navigation menu
2. Enter credentials:
   - Username: `admin`
   - Password: `admin123`
3. Click "Login" button
4. You should be redirected to the home page showing "Hello, admin!"

## Troubleshooting

### Issue: "Cannot connect to the API"

**Solution:**
1. Verify the Authentication API is running (`https://localhost:7001`)
2. Check the browser console (F12) for CORS errors
3. Verify `FreshNoodle.UI/wwwroot/appsettings.json` has the correct API URL

### Issue: "CORS policy error"

**Solution:**
The API CORS is configured to allow these origins:
- `https://localhost:5001`
- `http://localhost:5000`
- `https://localhost:5173`
- `http://localhost:5173`

If your UI is running on a different port, update `FreshNoodle.API.Authentication/Program.cs`:
```csharp
policy.WithOrigins(
    "https://localhost:5001",
    "http://localhost:5000",
    "https://localhost:5173",
    "http://localhost:5173",
    "https://localhost:YOUR_PORT")  // Add your port here
```

### Issue: "401 Unauthorized" when accessing products

**Solution:**
1. Make sure you're logged in
2. Check browser console - the JWT token should be in localStorage
3. Verify the token is being sent in the Authorization header

### Issue: Login returns "Invalid username or password"

**Verify the database was seeded:**
1. Stop the API
2. Delete the database (if using LocalDB):
   ```cmd
   sqllocaldb stop mssqllocaldb
   sqllocaldb delete mssqllocaldb
   sqllocaldb create mssqllocaldb
   ```
3. Restart the API - it will recreate and seed the database

## Testing Product Management

Once logged in:

1. Navigate to "Products" page
2. You should see 4 sample products:
   - Laptop - $999.99
   - Mouse - $29.99
   - Keyboard - $79.99
   - Monitor - $299.99

3. **Test Create:**
   - Click "Add New Product"
   - Fill in name, price, description
   - Click "Create"
   - Product should appear in the list

4. **Test Edit:**
   - Click "Edit" on any product
   - Modify the details
   - Click "Update"
   - Changes should be saved

5. **Test Delete:**
   - Click "Delete" on any product
   - Product should be removed

## API URLs Reference

| Service | HTTPS | HTTP |
|---------|-------|------|
| Authentication API | https://localhost:7001 | http://localhost:5001 |
| Product API | https://localhost:7003 | http://localhost:5003 |
| User API | https://localhost:7002 | http://localhost:5002 |
| Blazor UI | https://localhost:5173 | http://localhost:5173 |

**Note:** Only the Authentication API needs to be running for login and product management to work. The Product API is optional (if you want to run products on a separate service).

## Sample Credentials

| Username | Password | Email |
|----------|----------|-------|
| admin | admin123 | admin@FreshNoodle.com |
| testuser | test123 | test@FreshNoodle.com |

## Verifying the Database

To check if the database was created and seeded correctly:

1. Connect to SQL Server LocalDB:
   ```cmd
   sqlcmd -S "(localdb)\mssqllocaldb" -d FreshNoodleDb
   ```

2. Check users:
   ```sql
   SELECT * FROM Users;
   GO
   ```

3. Check products:
   ```sql
   SELECT * FROM Products;
   GO
   ```

4. Exit:
   ```sql
   EXIT
   ```

## Success Checklist

- [ ] Authentication API starts on https://localhost:7001
- [ ] Can login with curl command successfully
- [ ] Blazor UI starts without errors
- [ ] UI appsettings.json points to https://localhost:7001
- [ ] Can navigate to login page in browser
- [ ] Can login with admin/admin123
- [ ] Home page shows "Hello, admin!"
- [ ] Products page shows 4 sample products
- [ ] Can create/edit/delete products

## Need Help?

If you're still experiencing issues:
1. Check both API and UI console outputs for errors
2. Check browser developer console (F12) for JavaScript errors
3. Verify all files are saved
4. Try rebuilding: `dotnet build FreshNoodle.sln`
5. Try clearing browser cache and localStorage
