using Microsoft.EntityFrameworkCore;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class AdminRepository : BaseRepository<Application.DTOs.Admin>, IAdminRepository
{
    public AdminRepository(StoreDbContext context) : base(context) { }

    public async Task<Application.DTOs.Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username && c.Activity.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}
