using Mapster;

namespace WebStore.CartApp.Profiles;

public sealed class CustomerProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Customer, CartDomain.Entities.Customer>()
            .ConstructUsing(src => CartDomain.Entities.Customer.Create(src.Username))
            .TwoWays();
    }
}