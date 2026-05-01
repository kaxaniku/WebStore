using Mapster;
using MapsterMapper;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Profiles;

namespace WebStore.CatalogTests;

public abstract class BaseTests
{
    private StoreDbContext _context;
    protected IUnitOfWork? _unitOfWork;
    protected IMapper _mapper = null!;
    protected CancellationTokenSource _cts;

    [SetUp]
    public virtual void SetUp()
    {
        _context = new StoreDbContext();
        _unitOfWork = new UnitOfWork(_context);
        _cts = new CancellationTokenSource();

        var config = new TypeAdapterConfig();
        config.Scan(typeof(CategoryProfile).Assembly);
        _mapper = new Mapper(config);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
        _cts.Dispose();
    }
}