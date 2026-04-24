using AutoMapper;
using WebStore.Application.Interfaces.Repositories;
using WebStore.Application.Interfaces.Services;
using WebStore.Domain;

namespace WebStore.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper, IProductService productService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productService = productService;
    }

    public async Task<Cart> GetCartAsync(int customerId, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct);

        if (cartDto == null)
        {
            var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
                ?? throw new KeyNotFoundException("Customer not found");

            var customerEntity = _mapper.Map<Customer>(customerDto);
            var newCartEntity = Cart.Create(customerEntity);

            var newCartDto = _mapper.Map<DTOs.Cart>(newCartEntity);
            await _unitOfWork.CartRepository.InsertAsync(newCartDto, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return newCartEntity;
        }

        return _mapper.Map<Cart>(cartDto);
    }

    public async Task AddToCartAsync(int customerId, int productId, int quantity, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException("Product not found");

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var productEntity = _mapper.Map<Product>(productDto);

        var existing = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productEntity.Id);

        var cartItemEntity = Cart.AddOrUpdateItem(cartEntity, productEntity, quantity);

        var cartItemDto = _mapper.Map<DTOs.CartItem>(cartItemEntity);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            await _productService.UpdateProductStockAsync(productEntity.Id, productEntity.Stock - quantity, ct);

            if (existing == null)
            {
                await _unitOfWork.CartItemRepository.InsertAsync(cartItemDto, ct);
            }
            else
            {
                await _unitOfWork.CartItemRepository.UpdateAsync(cartItemDto);
            }
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task RemoveFromCartAsync(int customerId, int productId, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var itemToRemove = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productId);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            if (itemToRemove != null)
            {
                await _productService.UpdateProductStockAsync(itemToRemove.Product.Id, itemToRemove.Product.Stock + itemToRemove.Quantity, ct);
                var cartItemDto = _mapper.Map<DTOs.CartItem>(itemToRemove);

                _unitOfWork.CartItemRepository.Delete(cartItemDto);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task IncreaseCartItemQuantityAsync(int customerId, int productId, int quantity, CancellationToken ct)
    {
        await AddToCartAsync(customerId, productId, quantity, ct);
    }

    public async Task DecreaseCartItemQuantityAsync(int customerId, int productId, int quantity, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var item = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productId);

        if (item != null)
        {
            item = Cart.CartItem.RemoveQuantity(item, quantity);
            await _productService.UpdateProductStockAsync(item.Product.Id, item.Product.Stock + quantity, ct);

            var cartItemDto = _mapper.Map<DTOs.CartItem>(item);
            await _unitOfWork.CartItemRepository.UpdateAsync(cartItemDto);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task ClearCartAsync(int customerId, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        var cartEntity = _mapper.Map<Cart>(cartDto);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            foreach (var item in cartEntity.Items)
            {
                var productEntity = Product.UpdateStock(item.Product, item.Product.Stock + item.Quantity);
                await _productService.UpdateProductStockAsync(productEntity.Id, productEntity.Stock, ct);
                var cartItemDto = _mapper.Map<DTOs.CartItem>(item);
                _unitOfWork.CartItemRepository.Delete(cartItemDto);
            }

            cartEntity = Cart.Clear(cartEntity);
            var cartDtoToUpdate = _mapper.Map<DTOs.Cart>(cartEntity);
            await _unitOfWork.CartRepository.UpdateAsync(cartDtoToUpdate);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<decimal> GetCartTotalAsync(int customerId, CancellationToken ct)
    {
        var cart = await GetCartAsync(customerId, ct);
        return cart.TotalAmount;
    }
}