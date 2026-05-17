using Mapster;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Profiles;

public sealed class CartProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Cart, Cart>()
            .ConstructUsing(src => Cart.Create(
                src.Customer.Adapt<Customer>()
                ))
            .TwoWays();
    }
}