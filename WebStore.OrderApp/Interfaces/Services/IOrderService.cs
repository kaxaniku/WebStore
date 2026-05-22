using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Interfaces.Services
{
    public interface IOrderService
    {
        Task CancelOrderAsync(int customerId, int orderId, CancellationToken ct);
        Task<Order> CreateOrderAsync(int customerId, CancellationToken ct);
        Task<Order?> GetMyOrderByIdAsync(int customerId, int orderId, CancellationToken ct);
        Task<IEnumerable<Order>> GetMyOrdersAsync(int customerId, CancellationToken ct);
        Task<int> PlaceOrderAsync(int customerId, CancellationToken ct);
    }
}