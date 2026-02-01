using System.Net.Http.Json;
using FreshNoodle.UI.Models;

namespace FreshNoodle.UI.Services;

public class CustomerApiService
{
    private readonly HttpClient _httpClient;

    public CustomerApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Master Data Methods
    public async Task<List<CustomerTypeViewModel>> GetCustomerTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CustomerTypeViewModel>>("api/customer-types") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AuthResponseModel> CreateCustomerTypeAsync(CustomerTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/customer-types", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create customer type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<AuthResponseModel> UpdateCustomerTypeAsync(int id, CustomerTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/customer-types/{id}", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to update customer type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<List<PriceTypeViewModel>> GetPriceTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PriceTypeViewModel>>("api/price-types") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AuthResponseModel> CreatePriceTypeAsync(PriceTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/price-types", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create price type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<AuthResponseModel> UpdatePriceTypeAsync(int id, PriceTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/price-types/{id}", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to update price type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<List<PaymentTypeViewModel>> GetPaymentTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PaymentTypeViewModel>>("api/payment-types") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AuthResponseModel> CreatePaymentTypeAsync(PaymentTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/payment-types", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create payment type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<AuthResponseModel> UpdatePaymentTypeAsync(int id, PaymentTypeViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/payment-types/{id}", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to update payment type" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    // Customer Methods
    public async Task<List<CustomerViewModel>> GetCustomersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/customers") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AuthResponseModel> CreateCustomerAsync(CustomerViewModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/customers", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to create customer" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }

    public async Task<AuthResponseModel> UpdateCustomerAsync(int id, CustomerViewModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/customers/{id}", model);
            return await response.Content.ReadFromJsonAsync<AuthResponseModel>() ?? new AuthResponseModel { Success = false, Message = "Failed to update customer" };
        }
        catch (Exception ex) { return new AuthResponseModel { Success = false, Message = ex.Message }; }
    }
}

public class CustomerTypeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PriceTypeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultPricePerKg { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PaymentTypeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CustomerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public string? CustomerTypeName { get; set; }
    public int PriceTypeId { get; set; }
    public string? PriceTypeName { get; set; }
    public int PaymentTypeId { get; set; }
    public string? PaymentTypeName { get; set; }
    public bool SkipDay { get; set; }
    public int? PriorityOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
