using MapsterMapper;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public static event Action<Product>? ProductAdded;
    public static event Action<Product>? ProductUpdated;
    public static event Action<int>? ProductRemoved;
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateProductAsync(string name, decimal price, string? description, int quantity, int categoryId, CancellationToken ct)
    {
        var productEntity = Product.Create(name, price, description, quantity, categoryId);
        var productDto = _mapper.Map<DTOs.Product>(productEntity);

        await _unitOfWork.ProductRepository.InsertAsync(productDto, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        Product.SetId(productEntity, productDto.Id);
        OnProductAdded(productEntity);
        return productDto.Id;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct)
    {
        var products = await _unitOfWork.ProductRepository.QueryAsync(x => x.Activity.IsActive, ct);
        return _mapper.Map<IEnumerable<Product>>(products);
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken ct)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct);
        if (product == null || !product.Activity.IsActive)
            return null;
        return _mapper.Map<Product?>(product);
    }

    public async Task UpdateProductAsync(int id, string name, string? description, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateDesc(entity, name, description);

        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
        OnProductUpdated(entity);
    }

    public async Task DeleteProductAsync(int id, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        _unitOfWork.ProductRepository.Delete(productDto);
        await _unitOfWork.SaveChangesAsync(ct);
        OnProductRemoved(productDto.Id);
    }
    public async Task<IEnumerable<Product>> SearchProductsAsync(string productName, CancellationToken ct)
    {
        var products = await _unitOfWork.ProductRepository.QueryAsync(p => p.Name.Contains(productName), ct);
        return _mapper.Map<IEnumerable<Product>>(products);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken ct)
    {
        var products = await _unitOfWork.ProductRepository.QueryAsync(p => p.Category.Id == categoryId, ct);
        return _mapper.Map<IEnumerable<Product>>(products);
    }

    public async Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateStock(entity, newStock);

        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
        OnProductUpdated(entity);
    }

    public async Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        var entity = _mapper.Map<Product>(productDto);
        Product.UpdatePrice(entity, newPrice);

        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
        OnProductUpdated(entity);
    }

    public async Task UpdateProductCategory(int id, int categoryId, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateCategory(entity, categoryId);
        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
        OnProductUpdated(entity);
    }

    private static void OnProductAdded(Product customer)
    {
        ProductAdded?.Invoke(customer);
    }

    private static void OnProductUpdated(Product customer)
    {
        ProductUpdated?.Invoke(customer);
    }

    private static void OnProductRemoved(int customerId)
    {
        ProductRemoved?.Invoke(customerId);
    }
}