using MapsterMapper;
using WebStore.CartApp.Interfaces.Repositories;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Cart> GetCartAsync(int customerId, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");
        var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");
        cartDto.Customer = customerDto;
        return _mapper.Map<Cart>(cartDto);
    }

    public async Task AddToCartAsync(int customerId, int productId, int quantity, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");
        var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException("Product not found");
        var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");
        cartDto.Customer = customerDto;

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var productEntity = _mapper.Map<Product>(productDto);

        var existing = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productEntity.Id);

        var cartItemEntity = Cart.AddOrUpdateItem(cartEntity, productEntity, quantity);

        var cartItemDto = _mapper.Map<DTOs.CartItem>(cartItemEntity);
        cartItemDto.Cart = null;
        cartItemDto.Product = null;
        cartItemDto.CartId = cartDto.Id;

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

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
        var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");
        cartDto.Customer = customerDto;

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var itemToRemove = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productId);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            if (itemToRemove != null)
            {
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
        var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");
        cartDto.Customer = customerDto;

        var cartEntity = _mapper.Map<Cart>(cartDto);
        var item = cartEntity.Items.FirstOrDefault(i => i.Product.Id == productId);

        if (item != null)
        {
            item = Cart.CartItem.RemoveQuantity(item, quantity);

            var cartItemDto = _mapper.Map<DTOs.CartItem>(item);
            await _unitOfWork.CartItemRepository.UpdateAsync(cartItemDto);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task ClearCartAsync(int customerId, CancellationToken ct)
    {
        var cartDto = await _unitOfWork.CartRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Cart not found");
        var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");
        cartDto.Customer = customerDto;

        var cartEntity = _mapper.Map<Cart>(cartDto);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            foreach (var item in cartEntity.Items)
            {
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