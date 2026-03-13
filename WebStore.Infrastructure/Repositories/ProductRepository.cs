using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class ProductRepository : BaseRepository<Application.DTOs.Product>, IProductRepository
{
    public ProductRepository(StoreDbContext context) : base(context) { }
}
