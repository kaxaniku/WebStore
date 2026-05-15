using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Interfaces.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int customerId, int productId, int quantity, CancellationToken ct);
        Task ClearCartAsync(int customerId, CancellationToken ct);
        Task DecreaseCartItemQuantityAsync(int customerId, int productId, int quantity, CancellationToken ct);
        Task<Cart> GetCartAsync(int customerId, CancellationToken ct);
        Task<decimal> GetCartTotalAsync(int customerId, CancellationToken ct);
        Task IncreaseCartItemQuantityAsync(int customerId, int productId, int quantity, CancellationToken ct);
        Task RemoveFromCartAsync(int customerId, int productId, CancellationToken ct);
    }
}