using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure.Repositories;

internal class CategoryRepository : BaseRepository<Application.DTOs.Category>, ICategoryRepository
{
    public CategoryRepository(StoreDbContext context) : base(context) { }
}
