using MapsterMapper;
using WebStore.CartApp.Interfaces.Repositories;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Services;

public class CartProductService : ICartProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> CreateCartProductAsync(int id, decimal price, int quantity, CancellationToken ct)
    {
        var productEntity = Product.Create(price, quantity);
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

    public async Task DeleteCartProductAsync(int id, CancellationToken ct)
    {
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        _unitOfWork.ProductRepository.Delete(productDto);
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
}