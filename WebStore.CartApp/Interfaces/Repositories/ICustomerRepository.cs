using WebStore.CartApp.DTOs;

namespace WebStore.CartApp.Interfaces.Repositories;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
