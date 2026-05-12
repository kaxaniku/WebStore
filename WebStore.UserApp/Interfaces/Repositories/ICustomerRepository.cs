using WebStore.UserApp.DTOs;

namespace WebStore.UserApp.Interfaces.Repositories;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
