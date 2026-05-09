using Mapster;
using MapsterMapper;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Profiles;
using WebStore.CatalogApp.Services;

namespace WebStore.CatalogTests;

public abstract class BaseTests
{
    private CatalogDbContext _context;
    protected Mock<IPublishEndpoint>? _publishMock;
    protected IUnitOfWork? _unitOfWork;
    protected IMapper _mapper = null!;
    protected CancellationTokenSource _cts;

    [SetUp]
    public virtual void SetUp()
    {
        _publishMock = new Mock<IPublishEndpoint>();
        _context = new CatalogDbContext();
        _unitOfWork = new UnitOfWork(_context);
        _cts = new CancellationTokenSource();
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
        _cts.Dispose();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new TypeAdapterConfig();
        config.Scan(typeof(CategoryProfile).Assembly);
        _mapper = new Mapper(config);

        _context = new CatalogDbContext();

        _context.Database.ExecuteSqlRaw("DELETE FROM Products");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0)");

        _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Categories', RESEED, 0)");

        _context.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context = new CatalogDbContext();

        _context.Database.ExecuteSqlRaw("DELETE FROM Products");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0)");

        _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Categories', RESEED, 0)");

        _context.Dispose();
    }
}