using WebStore.UserApp.DTOs;

namespace WebStore.UserApp.Interfaces.Repositories;

public interface IAdminRepository : IBaseRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
