using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class CartItemRepository : BaseRepository<Application.DTOs.CartItem>, ICartItemRepository
{
    public CartItemRepository(StoreDbContext context) : base(context) { }
}
