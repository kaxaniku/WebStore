using Microsoft.EntityFrameworkCore.Storage;
using WebStore.CatalogApp.DTOs;
using WebStore.CatalogApp.Interfaces.Repositories;

namespace Webstore.CatalogInfrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private readonly Lazy<IProductRepository> _product;
    private readonly Lazy<ICategoryRepository> _category;

    public IProductRepository ProductRepository => CheckDisposedAndGet(_product);
    public ICategoryRepository CategoryRepository => CheckDisposedAndGet(_category);

    public UnitOfWork(CatalogDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _product = new Lazy<IProductRepository>(() => new ProductRepository(_context));
        _category = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
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

            _disposed = true;
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, GetType());

    ~UnitOfWork() => Dispose(false);
}
