using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.API.Product.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Core.Entities.Product> _productRepository;

    public ProductService(IRepository<Core.Entities.Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CreatedDate = product.CreatedDate
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            CreatedDate = p.CreatedDate
        });
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var product = new Core.Entities.Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Description = productDto.Description,
            CreatedDate = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);

        return new ProductDto
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
            Description = createdProduct.Description,
            CreatedDate = createdProduct.CreatedDate
        };
    }

    public async Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        product.Name = productDto.Name;
        product.Price = productDto.Price;
        product.Description = productDto.Description;

        var updatedProduct = await _productRepository.UpdateAsync(product);

        return new ProductDto
        {
            Id = updatedProduct.Id,
            Name = updatedProduct.Name,
            Price = updatedProduct.Price,
            Description = updatedProduct.Description,
            CreatedDate = updatedProduct.CreatedDate
        };
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}
