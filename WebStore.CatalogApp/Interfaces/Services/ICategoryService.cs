using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Interfaces.Services;

public interface ICategoryService
{
    Task<int> AddAsync(string catName, CancellationToken cancellationToken);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task RemoveAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(int id, string newName, CancellationToken cancellationToken);
}