using WebStore.OrderApp.Interfaces.Repositories;

namespace WebStore.OrderInfrastructure.Repositories;

internal class ProductRepository : BaseRepository<OrderApp.DTOs.Product>, IProductRepository
{
    public ProductRepository(OrderDbContext context) : base(context) { }
}
