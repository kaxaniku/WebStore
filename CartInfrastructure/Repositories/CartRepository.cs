using WebStore.CartApp.Interfaces.Repositories;

namespace WebStore.CartInfrastructure.Repositories;

internal class CartRepository : BaseRepository<CartApp.DTOs.Cart>, ICartRepository
{
    public CartRepository(CartDbContext context) : base(context) { }
}
