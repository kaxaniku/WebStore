using AutoMapper;

namespace WebStore.Application.Profiles;

public sealed class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<DTOs.Product, Domain.Product>().ReverseMap();
    }
}