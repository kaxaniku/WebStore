using WebStore.UserApp.Interfaces.Repositories;

namespace WebStore.UserInfrastructure.Repositories;

internal class UserRepository : BaseRepository<UserApp.DTOs.User>, IUserRepository
{
    public UserRepository(UserDbContext context) : base(context) { }
}
