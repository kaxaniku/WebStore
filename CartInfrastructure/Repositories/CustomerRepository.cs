using Microsoft.EntityFrameworkCore;
using WebStore.CartApp.Interfaces.Repositories;

namespace WebStore.CartInfrastructure.Repositories;

internal class CustomerRepository : BaseRepository<CartApp.DTOs.Customer>, ICustomerRepository
{
    public CustomerRepository(CartDbContext context) : base(context) { }

    public async Task<CartApp.DTOs.Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username && c.Activity.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}
