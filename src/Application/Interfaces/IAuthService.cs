using System.Security.Claims;

namespace Application.Interfaces;

public interface IAuthService
{
    string GenerateJWTToken(List<Claim> claims);
}