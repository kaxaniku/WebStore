using Mapster;
using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Profiles;

public sealed class CartProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Cart, CartDomain.Entities.Cart>()
            .ConstructUsing(src => CartDomain.Entities.Cart.Create(
                src.Customer.Adapt<Customer>()
                ))
            .TwoWays();
    }
}