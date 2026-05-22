using MapsterMapper;
using WebStore.OrderApp.Interfaces.Repositories;
using WebStore.OrderApp.Interfaces.Services;
using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IOrderCartService _cartService;
    private readonly IOrderProductService _productService;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderCartService cartService, IOrderProductService productService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cartService = cartService;
        _productService = productService;
    }

    public async Task<Order> CreateOrderAsync(int customerId, CancellationToken ct)
    {
        //var cartEntity = await _cartService.GetCartAsync(customerId, ct);
        //if (!cartEntity.Items.Any())
        //    throw new InvalidOperationException("Cannot create an order with an empty cart.");

        var orderEntity = Order.Create(customerId);

        return orderEntity;
    }

    public async Task<int> PlaceOrderAsync(int customerId, CancellationToken ct)
    {
        var orderEntity = await CreateOrderAsync(customerId, ct);

        var orderDto = _mapper.Map<DTOs.Order>(orderEntity);
        var orderItemDtos = _mapper.Map<IEnumerable<DTOs.OrderItem>>(orderEntity.Items);

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            await _unitOfWork.OrderRepository.InsertAsync(orderDto, ct);
            Order.SetId(orderEntity, orderDto.Id);

            foreach (var itemDto in orderItemDtos)
            {
                itemDto.Order = orderDto;
                await _unitOfWork.OrderItemRepository.InsertAsync(itemDto, ct);

                var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(itemDto.Product.Id, ct)
                    ?? throw new KeyNotFoundException($"Product with ID {itemDto.Product.Id} not found");
                var productEntity = _mapper.Map<Product>(productDto);
                Product.UpdateStock(productEntity, productEntity.Stock - itemDto.Quantity);
                await _productService.UpdateProductStockAsync(productEntity.Id, productEntity.Stock, ct);
            }

            await _cartService.ClearCartAsync(customerId, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            return orderDto.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetMyOrdersAsync(int customerId, CancellationToken ct)
    {
        var orderDtos = await _unitOfWork.OrderRepository.QueryAsync(o => o.CustomerId == customerId, ct);
        return _mapper.Map<IEnumerable<Order>>(orderDtos);
    }

    public async Task<Order?> GetMyOrderByIdAsync(int customerId, int orderId, CancellationToken ct)
    {
        var orderDto = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, ct);

        if (orderDto == null || orderDto.CustomerId != customerId)
            throw new KeyNotFoundException("Order not found or access denied.");

        return _mapper.Map<Order>(orderDto);
    }

    public async Task CancelOrderAsync(int customerId, int orderId, CancellationToken ct)
    {
        var orderDto = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        if (orderDto.CustomerId != customerId)
            throw new UnauthorizedAccessException("Cannot cancel an order belonging to another customer.");

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            var orderEntity = _mapper.Map<Order>(orderDto);

            //foreach (var item in orderEntity.Items)
            //{
            //    //var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(item.CartItem.Product.Id, ct);
            //    //if (productDto != null)
            //    //{
            //    //    var productEntity = _mapper.Map<Product>(productDto);
            //    //    Product.UpdateStock(productEntity, productEntity.Stock + item.Quantity);
            //    //    _mapper.Map(productEntity, productDto);
            //    }
            //}

            _unitOfWork.OrderRepository.Delete(orderDto);

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}