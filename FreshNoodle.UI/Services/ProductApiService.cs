using System.Net.Http.Json;
using FreshNoodle.UI.Models;

namespace FreshNoodle.UI.Services;

public class ProductApiService
{
    private readonly HttpClient _httpClient;

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductViewModel>> GetAllProductsAsync()
    {
        try
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductViewModel>>("api/products");
            return products ?? new List<ProductViewModel>();
        }
        catch
        {
            return new List<ProductViewModel>();
        }
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductViewModel>($"api/products/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateProductAsync(ProductViewModel product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", product);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateProductAsync(int id, ProductViewModel product)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", product);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
