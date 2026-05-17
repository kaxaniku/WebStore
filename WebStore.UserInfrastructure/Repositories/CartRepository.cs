using WebStore.UserApp.Interfaces.Repositories;

namespace WebStore.UserInfrastructure.Repositories;

internal class CartRepository : BaseRepository<UserApp.DTOs.Cart>, ICartRepository
{
    public CartRepository(UserDbContext context) : base(context) { }
}
