using Mapster;

namespace WebStore.OrderApp.Profiles;

public sealed class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DTOs.Product, OrderDomain.Entities.Product>()
            .ConstructUsing(src => OrderDomain.Entities.Product.Create(
                src.Name,
                src.Price,
                src.Stock
                ));
    }
}