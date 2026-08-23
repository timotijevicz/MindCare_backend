using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MentalHealth.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MentalHealth.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Korisnik> _userManager;

        public TokenService(IConfiguration config, UserManager<Korisnik> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public string CreateToken(Korisnik korisnik)
        {
            var roles = _userManager.GetRolesAsync(korisnik).Result;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, korisnik.Id),
                new Claim(ClaimTypes.Name, korisnik.UserName),
                new Claim(ClaimTypes.Email, korisnik.Email),
                new Claim("Ime", korisnik.Ime),
                new Claim("Prezime", korisnik.Prezime)
            };

            // Dodavanje uloga u token kao claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}