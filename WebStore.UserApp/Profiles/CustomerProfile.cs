using Mapster;

namespace WebStore.UserApp.Profiles;

public sealed class CustomerProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Customer, UserDomain.Entities.Customer>()
            .ConstructUsing(src => UserDomain.Entities.Customer.Create(src.Username, src.Email, src.PasswordHash))
            .TwoWays();
    }
}