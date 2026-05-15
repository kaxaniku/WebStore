using Mapster;

namespace WebStore.CartApp.Profiles;

public sealed class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Product, CartDomain.Entities.Product>()
            .ConstructUsing(src => CartDomain.Entities.Product.Create(
                src.Price,
                src.Stock
                ));
    }
}