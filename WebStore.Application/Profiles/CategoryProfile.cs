using AutoMapper;

namespace WebStore.Application.Profiles;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<DTOs.Category, Domain.Category>().ReverseMap();
    }
}