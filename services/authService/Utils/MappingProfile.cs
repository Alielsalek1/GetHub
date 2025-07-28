using AutoMapper;
using authService.Dtos;
using authService.models;
using authService.DTOs;

namespace authService.Utils;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuthUser, RegisterRequest>().ReverseMap();
        CreateMap<AuthUser, LoginResponse>().ReverseMap();
    }
}