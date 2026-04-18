using WebStore.Application.DTOs;

namespace WebStore.Application.Interfaces.Repositories;

public interface IAdminRepository : IBaseRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
