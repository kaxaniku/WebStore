namespace WebStore.CartApp.Interfaces.Services
{
    public interface ICartProductService
    {
        Task<int> CreateCartProductAsync(int id, string name, decimal price, int quantity, string imagePath, CancellationToken ct);
        Task DeleteCartProductAsync(int id, CancellationToken ct);
        Task UpdateProductNameAsync(int id, string newName, CancellationToken ct);
        Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct);
        Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct);
        Task UpdateProductImagePathAsync(int id, string newImagePath, CancellationToken ct);
    }
}