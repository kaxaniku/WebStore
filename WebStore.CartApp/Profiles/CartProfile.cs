using Mapster;
using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Profiles;

public sealed class CartProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Cart, Cart>()
            .ConstructUsing(src => Cart.Create(
                src.Customer.Adapt<Customer>(),
                src.Items.Adapt<List<Cart.CartItem>>()!
                ))
            .TwoWays();

        config.NewConfig<Cart.CartItem, DTOs.CartItem>()
            .Map(dest => dest.ProductId, src => src.Product.Id)
            .Map(dest => dest.CartId, src => src.CartId);

        config.NewConfig<DTOs.CartItem, Cart.CartItem>()
            .ConstructUsing(src => Cart.CartItem.Create(
                src.Product.Adapt<Product>()!,
                src.Quantity,
                src.CartId
                ))
            .TwoWays();
    }
}