using WebStore.CatalogApp.DTOs;
using WebStore.CatalogApp.Interfaces.Repositories;

namespace Webstore.CatalogInfrastructure.Repositories;

internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context) { }
}
