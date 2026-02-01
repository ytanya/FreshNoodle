using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FreshNoodle.UI;
using FreshNoodle.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with API base address
// Change this URL to your API endpoint (e.g., https://localhost:7001 for Authentication API)
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7001";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// Register authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());

// Register API services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<CustomerApiService>();


await builder.Build().RunAsync();
