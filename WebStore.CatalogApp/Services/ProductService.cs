using MapsterMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using R2StorageApp.Interfaces;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;
using WebStore.Contracts.Catalog.Product;

namespace WebStore.CatalogApp.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IStorageService _storageService;
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint, IStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _storageService = storageService;
    }

    public async Task<int> CreateProductAsync(string name, decimal price, string? description, int quantity, int categoryId, IFormFile imageFile, CancellationToken ct)
    {
        var productEntity = Product.Create(name, price, description, quantity, categoryId);
        var productDto = _mapper.Map<DTOs.Product>(productEntity);
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId, ct)
            ?? throw new KeyNotFoundException("Category not found");

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            if (imageFile != null)
            {
                await _storageService.DeleteFileAsync(productDto.ImagePath);

                var fn = Path.GetFileNameWithoutExtension(imageFile.FileName);
                using var stream = imageFile.OpenReadStream();
                string newFileName = $"Products/{fn}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

                await _storageService.UploadFileAsync(stream, newFileName, imageFile.ContentType);

                productDto.ImagePath = newFileName;
            }
            await _unitOfWork.ProductRepository.InsertAsync(productDto, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _publishEndpoint.Publish(new ProductCreated
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            });
            await _unitOfWork.SaveChangesAsync(ct);
            Product.SetId(productEntity, productDto.Id);
            await _unitOfWork.CommitAsync(ct);
            return productDto.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
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
        await _publishEndpoint.Publish(new ProductUpdated
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock
        });
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteProductAsync(int id, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        await _storageService.DeleteFileAsync(productDto.ImagePath);
        _unitOfWork.ProductRepository.Delete(productDto);
        await _publishEndpoint.Publish(new ProductDeleted(id), ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveProductsByCategoryIdAsync(int categoryId, CancellationToken ct)
    {
        var products = await _unitOfWork.ProductRepository.QueryAsync(p => p.Category.Id == categoryId && p.Activity.IsActive, ct);
        foreach (var product in products)
        {
            await _storageService.DeleteFileAsync(product.ImagePath);
            _unitOfWork.ProductRepository.Delete(product);
            await _publishEndpoint.Publish(new ProductDeleted(product.Id), ct);
        }
        await _unitOfWork.SaveChangesAsync(ct);
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
        await _publishEndpoint.Publish(new ProductUpdated
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateLocalProductStockAsync(int id, int newStock, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateStock(entity, newStock);

        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        var entity = _mapper.Map<Product>(productDto);
        Product.UpdatePrice(entity, newPrice);

        _mapper.Map(entity, productDto);
        await _publishEndpoint.Publish(new ProductUpdated
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateProductCategory(int id, int categoryId, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateCategory(entity, categoryId);
        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateProductImageAsync(int id, IFormFile imageFile, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        if (imageFile != null)
        {
            await _storageService.DeleteFileAsync(productDto.ImagePath);
            var fn = Path.GetFileNameWithoutExtension(imageFile.FileName);
            using var stream = imageFile.OpenReadStream();
            string newFileName = $"Products/{fn}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            await _storageService.UploadFileAsync(stream, newFileName, imageFile.ContentType);
            productDto.ImagePath = newFileName;
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}