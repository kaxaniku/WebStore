using WebStore.CatalogApp.DTOs;

namespace WebStore.CatalogApp.Interfaces.Repositories;

public interface IAdminRepository : IBaseRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
