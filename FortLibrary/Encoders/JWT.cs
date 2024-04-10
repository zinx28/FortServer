using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FortLibrary.Encoders
{
    public class JWT
    {
        public static string GenerateJwtToken(Claim[] claims, int expires)
        {
            var GrabBytes = new byte[32];
            RandomNumberGenerator.Fill(GrabBytes);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(GrabBytes), SecurityAlgorithms.HmacSha256Signature)
            });

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static string GenerateRandomJwtToken(int expires, string secret)
        {
            var GrabBytes = new byte[32];
            RandomNumberGenerator.Fill(GrabBytes);

            var signingCredentials = Encoding.UTF8.GetBytes(secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingCredentials), SecurityAlgorithms.HmacSha256Signature)
            });

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static bool VerifyJwtToken(string token, string secret) // string secretKey
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Convert.FromBase64String(secret);

            var GrabBytes = new byte[32];
            RandomNumberGenerator.Fill(GrabBytes);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                tokenHandler.ValidateToken(token, parameters, out _);
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
