namespace WebStore.CatalogApp.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    void Commit();
    Task CommitAsync(CancellationToken cancellationToken);
    void Rollback();
    Task RollbackAsync(CancellationToken cancellationToken);

    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
}
