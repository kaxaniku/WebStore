using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Interfaces.Services;

public interface ICategoryService
{
    static abstract event Action<Category>? CategoryAdded;
    static abstract event Action<int>? CategoryRemoved;
    static abstract event Action<Category>? CategoryUpdated;

    Task<int> AddAsync(string catName, CancellationToken cancellationToken);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task RemoveAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(int id, string newName, CancellationToken cancellationToken);
}