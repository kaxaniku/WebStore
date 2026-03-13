using Microsoft.EntityFrameworkCore.Storage;
using Webstore.Infrastructure;
using WebStore.Application.Interfaces;
using WebStore.Application.Interfaces.Repositories;
using WebStore.Infrastructure.Repositories;

namespace WebStore.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly StoreDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private readonly Lazy<IProductRepository> _product;
    private readonly Lazy<ICategoryRepository> _category;
    private readonly Lazy<IUserRepository> _user;
    private readonly Lazy<IAdminRepository> _admin;
    private readonly Lazy<ICustomerRepository> _customer;
    private readonly Lazy<IOrderRepository> _order;
    private readonly Lazy<IOrderItemRepository> _orderItem;
    private readonly Lazy<ICartRepository> _cart;
    private readonly Lazy<ICartItemRepository> _cartItem;

    public IProductRepository ProductRepository => CheckDisposedAndGet(_product);
    public ICategoryRepository CategoryRepository => CheckDisposedAndGet(_category);
    public IUserRepository UserRepository => CheckDisposedAndGet(_user);
    public IAdminRepository AdminRepository => CheckDisposedAndGet(_admin);
    public ICustomerRepository CustomerRepository => CheckDisposedAndGet(_customer);
    public IOrderRepository OrderRepository => CheckDisposedAndGet(_order);
    public IOrderItemRepository OrderItemRepository => CheckDisposedAndGet(_orderItem);
    public ICartRepository CartRepository => CheckDisposedAndGet(_cart);
    public ICartItemRepository CartItemRepository => CheckDisposedAndGet(_cartItem);

    public UnitOfWork(StoreDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _product = new Lazy<IProductRepository>(() => new ProductRepository(_context));
        _category = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
        _user = new Lazy<IUserRepository>(() => new UserRepository(_context));
        _admin = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _customer = new Lazy<ICustomerRepository>(() => new CustomerRepository(_context));
        _order = new Lazy<IOrderRepository>(() => new OrderRepository(_context));
        _orderItem = new Lazy<IOrderItemRepository>(() => new OrderItemRepository(_context));
        _cart = new Lazy<ICartRepository>(() => new CartRepository(_context));
        _cartItem = new Lazy<ICartItemRepository>(() => new CartItemRepository(_context));
    }

    public int SaveChanges()
    {
        ThrowIfDisposed();
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void BeginTransaction()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new ArgumentException("Transaction has already started");

        _transaction = _context.Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new ArgumentException("Transaction has already started");

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    }

    public void Commit()
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;

    }

    public void Rollback()
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    private T CheckDisposedAndGet<T>(Lazy<T> lazy)
    {
        ThrowIfDisposed();
        return lazy.Value;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (_transaction != null)
            {
                _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_product.IsValueCreated)
                _product.Value.Dispose();

            if (_category.IsValueCreated)
                _category.Value.Dispose();

            if (_user.IsValueCreated)
                _user.Value.Dispose();

            if (_admin.IsValueCreated)
                _admin.Value.Dispose();

            if (_customer.IsValueCreated)
                _customer.Value.Dispose();

            if (_order.IsValueCreated)
                _order.Value.Dispose();

            if (_orderItem.IsValueCreated)
                _orderItem.Value.Dispose();

            if (_cart.IsValueCreated)
                _cart.Value.Dispose();

            if (_cartItem.IsValueCreated)
                _cartItem.Value.Dispose();
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_product.IsValueCreated)
                await _product.Value.DisposeAsync();

            if (_category.IsValueCreated)
                await _category.Value.DisposeAsync();

            if (_user.IsValueCreated)
                await _user.Value.DisposeAsync();

            if (_admin.IsValueCreated)
                await _admin.Value.DisposeAsync();

            if (_customer.IsValueCreated)
                await _customer.Value.DisposeAsync();

            if (_order.IsValueCreated)
                await _order.Value.DisposeAsync();

            if (_orderItem.IsValueCreated)
                await _orderItem.Value.DisposeAsync();

            if (_cart.IsValueCreated)
                await _cart.Value.DisposeAsync();

            if (_cartItem.IsValueCreated)
                await _cartItem.Value.DisposeAsync();

            _disposed = true;
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, GetType());

    ~UnitOfWork() => Dispose(false);
}
