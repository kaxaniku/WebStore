using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class OrderRepository : BaseRepository<Application.DTOs.Order>, IOrderRepository
{
    public OrderRepository(StoreDbContext context) : base(context) { }
}
