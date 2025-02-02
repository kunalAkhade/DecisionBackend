using DecisionBackend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DecisionBackend.Services.Implementation
{
    public class JwtService:IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public string GenerateJwtToken(string username)
        {
            var jwtSecret = _configuration["ApplicationSettings:JWT_Secret"];
            var expirationMinutes = _configuration.GetValue<int>("ApplicationSettings:JWT_ExpirationMinutes");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
