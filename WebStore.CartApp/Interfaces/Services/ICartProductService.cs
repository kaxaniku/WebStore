namespace WebStore.CartApp.Interfaces.Services
{
    public interface ICartProductService
    {
        Task<int> CreateCartProductAsync(int id, decimal price, int quantity, CancellationToken ct);
        Task DeleteCartProductAsync(int id, CancellationToken ct);
        Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct);
        Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct);
    }
}