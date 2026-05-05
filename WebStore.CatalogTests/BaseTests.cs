using Mapster;
using MapsterMapper;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Profiles;
using WebStore.CatalogApp.Services;

namespace WebStore.CatalogTests;

public abstract class BaseTests
{
    private StoreDbContext _context;
    protected IPublishEndpoint _publisher;
    protected IUnitOfWork? _unitOfWork;
    protected IMapper _mapper = null!;
    protected ITestHarness _harness = null!;
    protected CancellationTokenSource _cts;

    [SetUp]
    public async virtual Task SetUp()
    {
        var serviceCollection = new ServiceCollection();

        var config = new TypeAdapterConfig();
        config.Scan(typeof(CategoryProfile).Assembly);

        serviceCollection.AddSingleton(config);
        serviceCollection.AddScoped<IMapper, Mapper>();

        serviceCollection.AddDbContext<StoreDbContext>();
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<CategoryService>();

        serviceCollection.AddMassTransitTestHarness(x =>
        {
            x.AddEntityFrameworkOutbox<StoreDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
                o.DisableInboxCleanupService();
            });
        });

        var provider = serviceCollection.BuildServiceProvider();
        _harness = provider.GetRequiredService<ITestHarness>();
        await _harness.Start();

        var scope = _harness.Scope;

        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        _context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
        _cts = new CancellationTokenSource();

        _mapper = _harness.Scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _harness.Stop();
        _unitOfWork?.Dispose();
        _context?.Dispose();
        _cts.Dispose();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _context = new StoreDbContext();

        _context.Database.ExecuteSqlRaw("DELETE FROM Products");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0)");

        _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Categories', RESEED, 0)");

        _context.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context = new StoreDbContext();

        _context.Database.ExecuteSqlRaw("DELETE FROM Products");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0)");

        _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Categories', RESEED, 0)");

        _context.Dispose();
    }

    protected int GetOutboxMessageCount()
    {
        return _context.Set<OutboxMessage>().Count();
    }

    protected IPublishEndpoint GetPublishEndpoint()
    {
        return _harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
    }
}