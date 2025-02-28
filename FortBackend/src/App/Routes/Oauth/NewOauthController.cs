using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.Encoders;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Misc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FortBackend.src.App.Routes.Oauth
{
    //auth/v1/oauth/token

    [ApiController]
    [Route("auth/v1/oauth/token")]
    public class NewOauthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> NewToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var e = JWT.GenerateJwtToken(new[]
                {
                    new Claim("clientId", "ec684b8c687f479fadea3cb2ad83f5c6"),
                    new Claim("role", "GameClient"),
                    new Claim("productId", "prod-fn"),
                    new Claim("iss", "eos"),
                    new Claim("env", "env"),
                    new Claim("organizationId", "o-aa83a0a9bc45e98c80c1b1c9d92e9e"),
                    new Claim("features", System.Text.Json.JsonSerializer.Serialize(new string[] { "AntiCheat", "Connect", "Ecom", "Inventories" }), JsonClaimValueTypes.JsonArray),
                    new Claim("deploymentId", "62a9473a2dca46b29ccf17577fcf42d7"),
                    new Claim("sandboxId", "fn"),
                    new Claim("tokenType", "clientToken"),
                    new Claim("exp", "1714598878"),
                    new Claim("iat", "1714595278"),
                    new Claim("jti", "ecbe3f222a8b4f679084d2a0e6476cf5"),
                }, 24, Saved.DeserializeConfig.JWTKEY);

                var response = new
                {
                    access_token = e,
                    token_type = "bearer",
                    expires_at = DateTimeOffset.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    features = new string[] { "AntiCheat", "Connect", "Ecom", "Inventories" },
                    organization_id = "o-aa83a0a9bc45e98c80c1b1c9d92e9e",
                    product_id = "prod-fn",
                    sandbox_id = "fn",
                    deployment_id = "62a9473a2dca46b29ccf17577fcf42d7",
                    expires_in = 3599
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { });
        }
    }
}
