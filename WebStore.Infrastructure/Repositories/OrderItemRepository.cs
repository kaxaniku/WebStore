using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class OrderItemRepository : BaseRepository<Application.DTOs.OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(StoreDbContext context) : base(context) { }
}
