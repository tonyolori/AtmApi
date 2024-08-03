using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateJWTToken(List<Claim> claims)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        string? jwtIssuer = _configuration["Jwt:Issuer"];
        byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = jwtIssuer,
            Audience = jwtIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),// why sha256
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddHours(7)

        };

        SecurityToken jwtToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(jwtToken);
    }

}