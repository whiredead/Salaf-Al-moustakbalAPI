using Microsoft.IdentityModel.Tokens;
using ProjectPfe.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectPfe.Services
{
    public class JWTService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _jwtKey;

        public JWTService(IConfiguration config)
        {
            this._config = config;
            // jwtKey is used for both encripting and decripting the JWT token
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"])); // create a key for us , transform the jwt key to bytes
        }

        public string CreateJWT(User user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Nom),
                new Claim(ClaimTypes.Surname, user.Prenom)
            };
            var creadentials = new SigningCredentials(this._jwtKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"])),
                SigningCredentials = creadentials,
                Issuer = _config["JWT:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);

        }
    }
}
