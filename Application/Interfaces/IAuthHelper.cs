using Domain.Models;

namespace Application.Interfaces;

public interface IAuthHelper
{
    string GenerateJWTToken(User user);
}