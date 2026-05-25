using MapsterMapper;
using MassTransit;
using WebStore.Contracts.Cart;
using WebStore.OrderApp.Interfaces.Repositories;
using WebStore.OrderApp.Interfaces.Services;
using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IOrderProductService _productService;
    private readonly IRequestClient<GetCartRequest> _cartRequestClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IOrderProductService productService,
        IRequestClient<GetCartRequest> cartRequestClient,
        IPublishEndpoint publishEndpoint
        )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productService = productService;
        _cartRequestClient = cartRequestClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Order> CreateOrderAsync(int customerId, CancellationToken ct)
    {
        var response = await _cartRequestClient.GetResponse<GetCartResponse>(
            new GetCartRequest(customerId),
            ct
        );

        var cartData = response.Message;

        if (!cartData.Items.Any())
            throw new InvalidOperationException("Cannot create an order with an empty cart.");

        var productIds = cartData.Items.Select(i => i.ProductId).Distinct().ToList();

        var productList = await _unitOfWork.ProductRepository.QueryAsync(p => productIds.Contains(p.Id), ct);
        var products = productList.ToDictionary(p => p.Id);

        var items = cartData.Items.Select(item =>
        {
            if (!products.TryGetValue(item.ProductId, out var productDto))
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");

            return Order.OrderItem.Create(0, item.Quantity, productDto!.Price, item.ProductId);
        }).ToList();
        var orderEntity = Order.Create(customerId, items);

        return orderEntity;
    }

    public async Task<int> PlaceOrderAsync(int customerId, CancellationToken ct)
    {
        var orderEntity = await CreateOrderAsync(customerId, ct);
        var orderDto = _mapper.Map<DTOs.Order>(orderEntity);
        orderDto.Items = null;

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            await _unitOfWork.OrderRepository.InsertAsync(orderDto, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            Order.SetId(orderEntity, orderDto.Id);

            var orderItemDtos = _mapper.Map<IEnumerable<DTOs.OrderItem>>(orderEntity.Items);

            foreach (var itemDto in orderItemDtos)
            {
                itemDto.OrderId = orderDto.Id;
                itemDto.Order = null;
                itemDto.Product = null;
                await _unitOfWork.OrderItemRepository.InsertAsync(itemDto, ct);

                var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(itemDto.ProductId, ct)
                    ?? throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found");
                var productEntity = _mapper.Map<Product>(productDto);
                Product.UpdateStock(productEntity, productEntity.Stock - itemDto.Quantity);
                productDto.Stock = productEntity.Stock;
                await _productService.UpdateMainProductStockAsync(productDto, productEntity, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await _publishEndpoint.Publish(new ClearCartRequest(customerId), ct);

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
        var orderDtos = await _unitOfWork.OrderRepository.QueryAsync(o => o.CustomerId == customerId, ct, o => o.Items!);
        return _mapper.Map<IEnumerable<Order>>(orderDtos);
    }

    public async Task<Order?> GetMyOrderByIdAsync(int customerId, int orderId, CancellationToken ct)
    {
        var orderDto = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, ct);

        if (orderDto == null || orderDto.CustomerId != customerId)
            throw new KeyNotFoundException("Order not found or access denied.");
        orderDto.Items = (await _unitOfWork.OrderItemRepository.QueryAsync(i => i.OrderId == orderId, ct, i => i.Product!)).ToList();

        return _mapper.Map<Order>(orderDto);
    }

    public async Task CancelOrderAsync(int customerId, int orderId, CancellationToken ct)
    {
        var orderDto = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        if (orderDto.CustomerId != customerId)
            throw new UnauthorizedAccessException("Cannot cancel an order belonging to another customer.");

        orderDto.Items = (await _unitOfWork.OrderItemRepository.QueryAsync(i => i.OrderId == orderDto.Id, ct, i => i.Product!)).ToList();

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            var orderEntity = _mapper.Map<Order>(orderDto);

            foreach (var item in orderEntity.Items)
            {
                var productDto = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId, ct);
                if (productDto != null)
                {
                    var productEntity = _mapper.Map<Product>(productDto);
                    Product.UpdateStock(productEntity, productEntity.Stock + item.Quantity);
                    await _productService.UpdateMainProductStockAsync(productDto, productEntity, ct);
                }
            }

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