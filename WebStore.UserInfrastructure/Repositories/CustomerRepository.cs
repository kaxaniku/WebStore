using Microsoft.EntityFrameworkCore;
using WebStore.UserApp.Interfaces.Repositories;

namespace WebStore.UserInfrastructure.Repositories;

internal class CustomerRepository : BaseRepository<UserApp.DTOs.Customer>, ICustomerRepository
{
    public CustomerRepository(UserDbContext context) : base(context) { }

    public async Task<UserApp.DTOs.Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await Query(c => c.Username == username && c.Activity.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}
