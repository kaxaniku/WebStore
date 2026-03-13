using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class CartRepository : BaseRepository<Application.DTOs.Cart>, ICartRepository
{
    public CartRepository(StoreDbContext context) : base(context) { }
}
