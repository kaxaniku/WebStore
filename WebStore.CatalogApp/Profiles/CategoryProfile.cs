using Mapster;

namespace WebStore.CatalogApp.Profiles;

public sealed class CategoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Category, CatalogDomain.Entities.Category>()
            .ConstructUsing(src => CatalogDomain.Entities.Category.Create(src.Name))
            .TwoWays();
    }
}