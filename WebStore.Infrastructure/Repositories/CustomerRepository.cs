using WebStore.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WebStore.Infrastructure.Repositories;

internal class CustomerRepository : BaseRepository<Application.DTOs.Customer>, ICustomerRepository
{
    public CustomerRepository(StoreDbContext context) : base(context) { }

    public async Task<Application.DTOs.Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username).FirstOrDefaultAsync(cancellationToken);
    }
}
