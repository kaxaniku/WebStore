using WebStore.OrderApp.Interfaces.Repositories;

namespace WebStore.OrderInfrastructure.Repositories;

internal class OrderItemRepository : BaseRepository<OrderApp.DTOs.OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(OrderDbContext context) : base(context) { }
}
