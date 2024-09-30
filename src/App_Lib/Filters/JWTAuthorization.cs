using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.App_Lib.Abstract;
using src.App_Lib.Cache;
using src.App_Lib.Tools;

namespace src.App_Lib.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class JWTAuthorization : Attribute, IAuthorizationFilter
{
    public JWTAuthorization() { }

    public void OnAuthorization(AuthorizationFilterContext filterContext)
    {
		var tokenService = filterContext.HttpContext.RequestServices.GetRequiredService<ITokenService>();

        if (filterContext != null)
        {
            Microsoft.Extensions.Primitives.StringValues authorizationToken;

            filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

            var tokenSend = authorizationToken.FirstOrDefault();

            if (tokenSend != null)
            {
                if (tokenService.ValidateToken(tokenSend, out string message, out _, out _))
                {
                    filterContext.HttpContext.Response.Headers.Append("Authorization", tokenSend);
                    filterContext.HttpContext.Response.Headers.Append("RequestStatus", "Authorized");
                    return;
                }
                else
                {
                    filterContext.HttpContext.Response.Headers.Append("Authorization", tokenSend);
                    filterContext.HttpContext.Response.Headers.Append("RequestStatus", "Unauthorized");
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unauthorized";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    filterContext.Result = new JsonResult("Unauthorized")
                    {
                        Value = new
                        {
                            Status = "Unauthorized",
                            Message = $"Provided token is invalid. {message}"
                        },
                    };
                }
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "TokenNotFound";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                filterContext.Result = new JsonResult("TokenNotFound")
                {
                    Value = new
                    {
                        Status = "ExpectationFailed",
                        Message = "Token is not provided"
                    },
                };
            }
        }
    }
}