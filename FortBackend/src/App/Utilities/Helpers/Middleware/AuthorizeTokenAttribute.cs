using Discord.Net;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.Encoders.JWTCLASS;
using FortLibrary.EpicResponses.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace FortBackend.src.App.Utilities.Helpers.Middleware
{
    public class AuthorizeTokenAttribute : Attribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var tokenArray = context.HttpContext.Request.Headers["Authorization"].ToString().Split("bearer ");

                if (tokenArray.Length > 0)
                {
                    var token = tokenArray.Length > 1 ? tokenArray[1] : "";

                    bool FoundAccount = GlobalData.AccessToken.Any(e => e.token == token) || GlobalData.ClientToken.Any(e => e.token == token) || GlobalData.RefreshToken.Any(e => e.token == token);

                    if (FoundAccount)
                    {
                        var accessToken = token.Replace("eg1~", "");
                        var handler = new JwtSecurityTokenHandler();
                        var DecodedToken = handler.ReadJwtToken(accessToken);
                        string[] tokenParts = DecodedToken.ToString().Split(".");

                        if(tokenParts.Length == 2)
                        {
                            var Payload = tokenParts[1];

                            TokenPayload tokenPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenPayload>(Payload)!;

                            if (tokenPayload != null)
                            {
                                if(!string.IsNullOrEmpty(tokenPayload.Sub)) {

                                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile(tokenPayload.Sub);

                                    if (profileCacheEntry != null)
                                    {
                                        context.HttpContext.Items["ProfileData"] = profileCacheEntry;
                                        context.HttpContext.Items["Payload"] = tokenPayload;
                                        context.HttpContext.Items["Token"] = token;
                                        // return back the profilecacheentry
                                        return;
                                    }
                                }
                                

                            }
                            else
                            {
                                throw new BaseError
                                {
                                    errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                                    errorMessage = $"Authentication failed for {context.HttpContext.Request.Path}",
                                    messageVars = new List<string> { $"{context.HttpContext.Request.Path}" },
                                    numericErrorCode = 1032,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = $"Authentication failed for {context.HttpContext.Request.Path}",
                                };
                            }
                        }
                    }
                }
            }
            catch (BaseError ex)
            {
                var jsonResult = JsonConvert.SerializeObject(BaseError.FromBaseError(ex));
                context.Result = new ContentResult
                {
                    Content = jsonResult,
                    ContentType = "application/json",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"[AuthorizeTokenAttribute:Verify] -> {ex.Message}");
            }
        }
    }
}
