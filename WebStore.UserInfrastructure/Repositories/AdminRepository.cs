using Microsoft.EntityFrameworkCore;
using WebStore.UserApp.Interfaces.Repositories;

namespace WebStore.UserInfrastructure.Repositories;

internal class AdminRepository : BaseRepository<UserApp.DTOs.Admin>, IAdminRepository
{
    public AdminRepository(UserDbContext context) : base(context) { }

    public async Task<UserApp.DTOs.Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username && c.Activity.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}
