using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _configuration;

        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJWTToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.AccountNumber.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtIssuer,
                Audience = jwtIssuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),// why sha256
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(7)

            };
               
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwtToken);
        }

    }
}





//getting random 32 bytes and place it there
//private string GenerateJWTToken(UserAccount user)
//{
//    // Your JWT token generation logic here
//    var claims = new List<Claim> {
//            new Claim(ClaimTypes.NameIdentifier, "user123"),
//            new Claim(ClaimTypes.Name, "John Doe"),
//        };

//    // Generate a key with sufficient length (256 bits) for HMAC-SHA256
//    var key = new byte[32]; // 32 bytes = 256 bits
//    using (var rng = RandomNumberGenerator.Create())
//    {
//        rng.GetBytes(key);
//    }

//    var jwtToken = new JwtSecurityToken(
//        claims: claims,
//        notBefore: DateTime.UtcNow,
//        expires: DateTime.UtcNow.AddDays(30),
//        signingCredentials: new SigningCredentials(
//            new SymmetricSecurityKey(key),
//            SecurityAlgorithms.HmacSha256Signature)
//    );

//    return new JwtSecurityTokenHandler().WriteToken(jwtToken);
//}
//    }