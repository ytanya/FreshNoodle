# Azure Deployment Guide (with Mobile Access)

This guide provides step-by-step instructions for deploying the FreshNoodle project (Blazor UI + 3 ASP.NET Core APIs + SQL Server) to Microsoft Azure, and ensuring it can be accessed by a mobile application (e.g., Expo/React Native).

## 1. Overview of Azure Resources

To deploy this architecture, you will need the following Azure services:
- **Azure SQL Database**: To host the `FreshNoodleDb`.
- **Azure App Service (Web Apps)**: 3 instances to host the Authentication API, User API, and Product API.
- **Azure Static Web Apps**: 1 instance to host the Blazor WebAssembly UI.

## 2. Database Deployment

1. **Create an Azure SQL Database** in the Azure Portal.
2. Ensure you configure the **Server firewall** to "Allow Azure services and resources to access this server" so your App Services can connect to it. Also, add your local client IP if you want to run migrations from your local machine.
3. Obtain the **ADO.NET Connection String** from the Azure Portal (it will look like `Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=yourdb;...`).
4. Apply migrations: 
   Update your local `appsettings.json` temporarily with the Azure connection string, then run:
   ```bash
   cd FreshNoodle.Infrastructure
   dotnet ef database update --startup-project ../FreshNoodle.API.Authentication
   ```

## 3. API Deployment & Configuration

### Update CORS for Cloud and Mobile
Currently, the APIs have hardcoded localhost URLs for CORS. For mobile apps (like an Expo app running on a physical device) and your Blazor UI hosted on Azure, you need to update the CORS policies in `Program.cs` for **all three APIs** (`FreshNoodle.API.Authentication`, `FreshNoodle.API.User`, `FreshNoodle.API.Product`).

Find the `AddCors` section in `Program.cs` and update it to something like this, which allows any origin (useful for mobile access) or specify your exact Azure UI URL:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // For mobile apps and unrestricted access
              .AllowAnyHeader()
              .AllowAnyMethod();
        
        // OR specify exact origins:
        // policy.WithOrigins("https://your-blazor-app.azurestaticapps.net")
        //       .AllowAnyHeader()
        //       .AllowAnyMethod()
        //       .AllowCredentials();
    });
});
```

### Deploy the APIs
For each API (`FreshNoodle.API.Authentication`, `FreshNoodle.API.User`, `FreshNoodle.API.Product`):
1. In Visual Studio, right-click the project -> **Publish**.
2. Target: **Azure** -> **Azure App Service (Windows or Linux)**.
3. Create a new App Service instance for each (e.g., `freshnoodle-auth-api`, `freshnoodle-user-api`, `freshnoodle-product-api`).
4. **Important Configuration**: In the Azure Portal for each App Service, go to **Configuration** -> **Application settings** and add:
   - Name: `ConnectionStrings:DefaultConnection`, Value: `[Your Azure SQL Connection String]`
5. Click **Publish**.

## 4. Blazor UI Deployment

1. Update the API Base Address for the cloud. Open `FreshNoodle.UI/wwwroot/appsettings.json` and change it to your deployed Authentication API URL:
   ```json
   {
     "ApiBaseAddress": "https://freshnoodle-auth-api.azurewebsites.net"
   }
   ```
2. In Visual Studio, right-click `FreshNoodle.UI` -> **Publish**.
3. Target: **Azure** -> **Azure Static Web Apps**.
4. Follow the prompts to connect to GitHub. This will generate a GitHub Actions workflow that automatically builds and deploys your Blazor WebAssembly app whenever you push to the repository.

## 5. Mobile Access (Expo/React Native/Native)

Mobile applications (iOS/Android) generally do not enforce CORS restrictions when compiled to native code, but if you are running in a WebView or using Expo Go during development, CORS matters. Setting `AllowAnyOrigin()` as shown in Step 3 solves this.

To connect your mobile app to the Azure-deployed backend:
1. In your mobile app's configuration/environment files, replace any `localhost` or `10.0.2.2` API URLs with the new Azure App Service URLs.
   ```javascript
   // Example config.js in your mobile app
   export const API_URLS = {
       auth: 'https://freshnoodle-auth-api.azurewebsites.net',
       user: 'https://freshnoodle-user-api.azurewebsites.net',
       product: 'https://freshnoodle-product-api.azurewebsites.net'
   };
   ```
2. Your mobile app can now make standard HTTP requests to these endpoints, authenticate, receive JWTs, and access exactly as it would locally.

## 6. Summary Checklist
- [ ] Azure SQL Database created and migrations applied.
- [ ] CORS policies updated in all 3 API `Program.cs` files to accommodate mobile/UI origination.
- [ ] Connection strings set in Azure App Service environment variables.
- [ ] Auth, User, and Product APIs published to Azure App Services.
- [ ] `ApiBaseAddress` updated in Blazor UI (`appsettings.json`).
- [ ] Blazor UI published to Azure Static Web Apps.
- [ ] Mobile app base URLs updated to point to the Azure App Service endpoints.
