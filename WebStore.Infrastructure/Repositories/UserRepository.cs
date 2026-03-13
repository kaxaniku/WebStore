using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class UserRepository : BaseRepository<Application.DTOs.User>, IUserRepository
{
    public UserRepository(StoreDbContext context) : base(context) { }
}
