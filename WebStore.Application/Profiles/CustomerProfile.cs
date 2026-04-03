using AutoMapper;

namespace WebStore.Application.Profiles;

public sealed class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<DTOs.Customer, Domain.Customer>().ReverseMap();
    }
}