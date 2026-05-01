using WebStore.Domain;

namespace WebStore.Application.Interfaces.Services
{
    public interface IProductService
    {
        static abstract event Action<Domain.Product>? ProductAdded;
        static abstract event Action<int>? ProductRemoved;
        static abstract event Action<Domain.Product>? ProductUpdated;

        Task<int> CreateProductAsync(string name, decimal price, string? description, int quantity, CancellationToken ct);
        Task DeleteProductAsync(int id, CancellationToken ct);
        Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken ct);
        Task<Product?> GetProductByIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<Product>> SearchProductsAsync(string productName, CancellationToken ct);
        Task UpdateProductAsync(int id, string name, string? description, CancellationToken ct);
        Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct);
        Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct);
    }
}