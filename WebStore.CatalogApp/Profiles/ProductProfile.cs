using Mapster;

namespace WebStore.CatalogApp.Profiles;

public sealed class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Product, CatalogDomain.Entities.Product>()
            .ConstructUsing(src => CatalogDomain.Entities.Product.Create(src.Name, src.Price, src.Description, src.Quantity))
            .TwoWays();
    }
}