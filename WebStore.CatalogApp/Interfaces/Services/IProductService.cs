using Microsoft.AspNetCore.Http;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Interfaces.Services;

public interface IProductService
{
    Task<int> CreateProductAsync(string name, decimal price, string? description, int quantity, int categoryId, IFormFile imageFile, CancellationToken ct);
    Task DeleteProductAsync(int id, CancellationToken ct);
    Task RemoveProductsByCategoryIdAsync(int categoryId, CancellationToken ct);
    Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken ct);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Product>> SearchProductsAsync(string productName, CancellationToken ct);
    Task UpdateProductAsync(int id, string name, string? description, CancellationToken ct);
    Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct);
    Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct);
    Task UpdateLocalProductStockAsync(int id, int newStock, CancellationToken ct);
    Task UpdateProductCategory(int id, int categoryId, CancellationToken ct);
    Task UpdateProductImageAsync(int id, IFormFile imageFile, CancellationToken ct);
}