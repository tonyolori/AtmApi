using AutoMapper;
using Domain.Entities;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Map properties with the same names
        CreateMap<User, UserDto>();
    }
}
