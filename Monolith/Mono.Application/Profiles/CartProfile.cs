using AutoMapper;

namespace WebStore.Application.Profiles;

public sealed class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<DTOs.Cart, Domain.Cart>().ReverseMap();
        CreateMap<DTOs.CartItem, Domain.Cart.CartItem>().ReverseMap();
    }
}