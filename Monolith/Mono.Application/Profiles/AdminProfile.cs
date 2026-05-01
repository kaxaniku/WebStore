using AutoMapper;

namespace WebStore.Application.Profiles;

public sealed class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<DTOs.Admin, Domain.Admin>().ReverseMap();
    }
}