using Mapster;

namespace WebStore.CatalogApp.Profiles;

public sealed class AdminProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Admin, CatalogDomain.Entities.Admin>()
            .ConstructUsing(src => CatalogDomain.Entities.Admin.Create(src.Username, src.PasswordHash))
            .TwoWays();
    }
}