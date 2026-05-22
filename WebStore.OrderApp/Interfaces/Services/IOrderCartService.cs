using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Interfaces.Services
{
    public interface IOrderCartService
    {
        //Task<Cart> GetCartAsync(int customerId, CancellationToken ct);
        Task ClearCartAsync(int customerId, CancellationToken ct);
    }
}