using WebStore.CartApp.Interfaces.Repositories;

namespace WebStore.CartInfrastructure.Repositories;

internal class ProductRepository : BaseRepository<CartApp.DTOs.Product>, IProductRepository
{
    public ProductRepository(CartDbContext context) : base(context) { }
}
