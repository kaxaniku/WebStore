using Microsoft.EntityFrameworkCore;
using WebStore.CatalogApp.DTOs;
using WebStore.CatalogApp.Interfaces.Repositories;

namespace Webstore.CatalogInfrastructure.Repositories;

internal class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    public AdminRepository(CatalogDbContext context) : base(context) { }

    public async Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username && c.Activity.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}
