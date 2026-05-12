using Mapster;

namespace WebStore.UserApp.Profiles;

public sealed class AdminProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Admin, UserDomain.Entities.Admin>()
            .ConstructUsing(src => UserDomain.Entities.Admin.Create(src.Username, src.Email, src.PasswordHash))
            .TwoWays();
    }
}