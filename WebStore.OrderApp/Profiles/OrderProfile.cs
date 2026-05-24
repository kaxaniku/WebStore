using Mapster;
using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Profiles;

public sealed class OrderProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Order, Order>()
            .ConstructUsing(src => Order.Create(
                src.CustomerId,
                src.Items.Adapt<List<Order.OrderItem>>()!
                ))
            .TwoWays();

        config.NewConfig<Order.OrderItem, DTOs.OrderItem>()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.OrderId, src => src.OrderId);

        config.NewConfig<DTOs.OrderItem, Order.OrderItem>()
            .ConstructUsing(src => Order.OrderItem.Create(
                src.OrderId,
                src.Quantity,
                src.OrderId,
                src.ProductId
                ))
            .TwoWays();
    }
}