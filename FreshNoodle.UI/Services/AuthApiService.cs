using System.Net.Http.Json;
using FreshNoodle.UI.Models;

namespace FreshNoodle.UI.Services;

public class AuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateProvider _authStateProvider;

    public AuthApiService(HttpClient httpClient, AuthStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponseModel> LoginAsync(LoginViewModel loginModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

            if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
            {
                await _authStateProvider.MarkUserAsAuthenticated(result.Token, result.User?.Username ?? "");
            }

            return result ?? new AuthResponseModel { Success = false, Message = "Login failed" };
        }
        catch (Exception ex)
        {
            return new AuthResponseModel { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VerifyResetCodeAsync(string email, string code)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/verify-code", new { Email = email, Code = code });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<AuthResponseModel> ResetPasswordWithCodeAsync(ResetPasswordViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password-with-code", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Reset failed" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
        try
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }
        catch { /* Ignore network errors on logout */ }
    }

    // Admin Methods
    public async Task<List<UserViewModel>> GetAllUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/admin/users") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<RoleViewModel>> GetRolesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoleViewModel>>("api/admin/roles") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AuthResponseModel> CreateUserAsync(CreateUserViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/users", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create user" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<bool> UpdateUserAsync(int userId, UpdateUserViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/users/{userId}", model);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/admin/users/{userId}/reset-password", new { UserId = userId, NewPassword = newPassword });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/users/{userId}/roles", roleIds);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<AuthResponseModel> CreateRoleAsync(RoleViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/roles", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create role" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<AuthResponseModel> UpdateRoleAsync(int id, RoleViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/roles/{id}", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to update role" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }


    public async Task<DashboardStatsViewModel?> GetDashboardStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardStatsViewModel>("api/dashboard/stats");
        }
        catch { return null; }
    }

    public async Task<FinancialStatsViewModel?> GetFinancialStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<FinancialStatsViewModel>("api/dashboard/financials");
        }
        catch { return null; }
    }

    public async Task<OperationsStatsViewModel?> GetOperationsStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OperationsStatsViewModel>("api/dashboard/operations");
        }
        catch { return null; }
    }
}



public class DashboardStatsViewModel
{
    public int ActiveCustomerCount { get; set; }
    public int ActiveUserCount { get; set; }
    public bool IsTodayClosed { get; set; }
    public int InactivePaymentTypeCount { get; set; }
    public int CustomersWithNoPriceTypeCount { get; set; }
}

public class FinancialStatsViewModel
{
    public decimal ExpectedRevenue { get; set; }
    public decimal ActualCollected { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal OverdueRevenue { get; set; }
    public int UnpaidCustomers { get; set; }
    public List<RevenuePointViewModel> RevenueHistory { get; set; } = new();
    public List<RevenuePointViewModel> TodayRevenueHistory { get; set; } = new();
}

public class RevenuePointViewModel
{
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class OperationsStatsViewModel
{
    public int TotalProduced { get; set; }
    public int RetailReserved { get; set; }
    public List<CustomerDeliveryViewModel> Deliveries { get; set; } = new();
}

public class CustomerDeliveryViewModel
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int? Priority { get; set; }
}

public class UserViewModel


{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class RoleViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateUserViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = new();
}

public class UpdateUserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}
