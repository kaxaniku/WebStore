using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class CustomerRepository : BaseRepository<Application.DTOs.Customer>, ICustomerRepository
{
    public CustomerRepository(StoreDbContext context) : base(context) { }
}
