using WebStore.CartApp.Interfaces.Repositories;

namespace WebStore.CartInfrastructure.Repositories;

internal class CartItemRepository : BaseRepository<CartApp.DTOs.CartItem>, ICartItemRepository
{
    public CartItemRepository(CartDbContext context) : base(context) { }
}
