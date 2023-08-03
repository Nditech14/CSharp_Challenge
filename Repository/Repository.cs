
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Core.Repository
{
    public class Repository : IRepository
    {
        public object GenerateToken(string username,
          string email, IConfiguration config, string[] roles)
        {
            // create and setup claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
            };

            // create and setup roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            // create security token descriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:JWTSigningkey").Value)),
                SecurityAlgorithms.HmacSha256Signature)
            };
            // create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenCreated = tokenHandler.CreateToken(securityTokenDescriptor);

            return new
            {
                token = tokenHandler.WriteToken(tokenCreated).ToString(),
            };
        }


    }
}
