using MapsterMapper;
using MassTransit;
using WebStore.Contracts.Catalog.Product;
using WebStore.OrderApp.Interfaces.Repositories;
using WebStore.OrderApp.Interfaces.Services;
using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Services;

public class OrderProductService : IOrderProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderProductService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> CreateOrderProductAsync(int id, string name, decimal price, int quantity, CancellationToken ct)
    {
        var existingProductDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct);

        if (existingProductDto != null)
        {
            return existingProductDto.Id;
        }

        var productEntity = Product.Create(name, price, quantity);
        Product.SetId(productEntity, id);
        var productDto = _mapper.Map<DTOs.Product>(productEntity);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            await _unitOfWork.ProductRepository.InsertAsync(productDto, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
            return productDto.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task DeleteOrderProductAsync(int id, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        _unitOfWork.ProductRepository.Delete(productDto);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateProductNameAsync(int id, string newName, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        var entity = _mapper.Map<Product>(productDto);
        Product.UpdateName(entity, newName);
        _mapper.Map(entity, productDto);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct)
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
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateMainProductStockAsync(DTOs.Product productDto, Product productEntity, CancellationToken ct)
    {
        await _publishEndpoint.Publish(new ProductUpdated
        {
            Id = productDto.Id,
            Name = productEntity.Name,
            Price = productEntity.Price,
            Stock = productEntity.Stock
        }, ct);
    }
}