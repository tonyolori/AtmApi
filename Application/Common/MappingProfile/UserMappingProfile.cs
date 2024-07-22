using AutoMapper;
using Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Map properties with the same names
        CreateMap<User, UserDto>();
    }
}
