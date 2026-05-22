using WebStore.OrderApp.Interfaces.Repositories;

namespace WebStore.OrderInfrastructure.Repositories;

internal class OrderRepository : BaseRepository<OrderApp.DTOs.Order>, IOrderRepository
{
    public OrderRepository(OrderDbContext context) : base(context) { }
}
