using Microsoft.EntityFrameworkCore.Storage;
using WebStore.UserApp.Interfaces.Repositories;

namespace WebStore.UserInfrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly UserDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private readonly Lazy<IUserRepository> _user;
    private readonly Lazy<IAdminRepository> _admin;
    private readonly Lazy<ICustomerRepository> _customer;

    public IUserRepository UserRepository => CheckDisposedAndGet(_user);
    public IAdminRepository AdminRepository => CheckDisposedAndGet(_admin);
    public ICustomerRepository CustomerRepository => CheckDisposedAndGet(_customer);

    public UnitOfWork(UserDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _user = new Lazy<IUserRepository>(() => new UserRepository(_context));
        _admin = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _customer = new Lazy<ICustomerRepository>(() => new CustomerRepository(_context));
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

            if (_user.IsValueCreated)
                _user.Value.Dispose();

            if (_admin.IsValueCreated)
                _admin.Value.Dispose();

            if (_customer.IsValueCreated)
                _customer.Value.Dispose();
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

            if (_user.IsValueCreated)
                await _user.Value.DisposeAsync();

            if (_admin.IsValueCreated)
                await _admin.Value.DisposeAsync();

            if (_customer.IsValueCreated)
                await _customer.Value.DisposeAsync();

            _disposed = true;
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, GetType());

    ~UnitOfWork() => Dispose(false);
}
