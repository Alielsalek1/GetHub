using AutoMapper;
using authService.Dtos;
using authService.models;

namespace authService.Utils;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuthUser, RegisterRequest>().ReverseMap();
        CreateMap<AuthUser, UserResponse>().ReverseMap();
    }
}