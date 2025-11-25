using FortLibrary.Encoders.JWTCLASS;
using FortLibrary.Shop;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FortLibrary.Encoders
{
    public class JWT
    {
        /// <summary>
        /// Generates the JWT Token from the given claims and expiration time
        /// </summary>
        /// <param name="claims">An array of claims that will be included in the JWT token.</param>
        /// <param name="expires">The expiration time in hours for the JWT token.</param>
        /// <param name="secret">The secret key used to sign the JWT token.</param>
        /// <returns>The JWT token as a string</returns>
        public static string GenerateJwtToken(Claim[] claims, int expires, string secret)
        {
            var GrabBytes = new byte[32];
            RandomNumberGenerator.Fill(GrabBytes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expires),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            });

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        /// <summary>
        /// Generates a random JWT Token from the given expiration time
        /// </summary>
        /// <param name="expires">The expiration time in hours for the JWT token.</param>
        /// <param name="secret">The secret key used to sign the JWT token.</param>
        /// <returns>The JWT token as a string</returns>
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

        /// <summary>
        /// Verifies the given token by a 'secret'
        /// </summary>
        /// <param name="token">The jwt token string to verify.</param>
        /// <param name="secret">The secret key used to validate the JWT token.</param>
        /// <returns>Returns true if the token is valid, otherwise false.</returns>
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

            // todo redo this
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

        /// <summary>
        /// Checks if the token is expired
        /// </summary>
        /// <param name="token">The jwt token string to verify.</param>
        /// <returns>Returns true if the token is expired, otherwise false.</returns>
        public static bool VerifyTokenExpired(string token)
        {
            var accessToken = token.Replace("eg1~", "");
            var handler = new JwtSecurityTokenHandler();
            var DecodedToken = handler.ReadJwtToken(accessToken);
            string[] tokenParts = DecodedToken.ToString().Split(".");

            if (tokenParts.Length == 2)
            {
                var Payload = tokenParts[1];

                TokenPayload tokenPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenPayload>(Payload)!;

                if (tokenPayload != null && !string.IsNullOrEmpty(tokenPayload.Sub))
                {
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(tokenPayload.Exp.Value);
                    if (DateTimeOffset.UtcNow >= expTime)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
