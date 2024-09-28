using System;
using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.App_Lib.Tools;

namespace src.App_Lib.Filters;

// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authorization/Core/src/AuthorizeAttribute.cs

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class JWTAuthorization : Attribute, IAuthorizationFilter
{
    public JWTAuthorization() { }

    /// <summary>
    /// JWT Token should be present in header as 
    /// Key: "Authorization" Value: "Bearer jwt.token.here" (set both key and value without quotes)
    /// </summary>
    public void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        if (filterContext != null)
        {
            Microsoft.Extensions.Primitives.StringValues authorizationToken;

            filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

            var tokenSend = authorizationToken.FirstOrDefault();

            if (tokenSend != null)
            {
                if (ValidateToken(tokenSend, out string message))
                {
                    filterContext.HttpContext.Response.Headers.Add("Authorization", tokenSend);
                    filterContext.HttpContext.Response.Headers.Add("RequestStatus", "Authorized");
                    return;
                }
                else
                {
                    filterContext.HttpContext.Response.Headers.Add("Authorization", tokenSend);
                    filterContext.HttpContext.Response.Headers.Add("RequestStatus", "Unauthorized");
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

    public bool ValidateToken(string token, out string message)
    {
        if (token.StartsWith("Bearer ")) token = token.Split("Bearer ")[1];

        JWTFactory jwt = new JWTFactory(
            issuer: "",
            signKey: "",
            encryptKey: ""
            );

        bool isValid = jwt.ValidateToken(token, out message, out _);

        return isValid;
    }
}

