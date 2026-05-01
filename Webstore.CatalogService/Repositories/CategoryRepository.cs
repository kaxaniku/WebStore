using WebStore.CatalogApp.DTOs;
using WebStore.CatalogApp.Interfaces.Repositories;

namespace Webstore.CatalogInfrastructure.Repositories;

internal class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(StoreDbContext context) : base(context) { }
}
