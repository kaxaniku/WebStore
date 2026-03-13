using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Application.Interfaces;

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
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IAdminRepository AdminRepository { get; }
    IUserRepository UserRepository { get; }
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }
}
