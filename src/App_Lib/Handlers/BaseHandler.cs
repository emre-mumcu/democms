using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Requirements;

namespace src.App_Lib.Handlers;

public class BaseHandler : AuthorizationHandler<BaseRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Use a handler for multiple requirements
    //public override async Task HandleAsync(AuthorizationHandlerContext context)
    //{
    //    var pendingRequirements = context.PendingRequirements.ToList();

    //    foreach (var r in pendingRequirements)
    //    {
    //        if (r is BaseRequirement)
    //        {

    //            context.Succeed(r);

    //        }

    //    }
    //}

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BaseRequirement requirement)
    {
        //if (context.Resource is HttpContext httpContext)
        //{
        //    var endpoint = httpContext.GetEndpoint();
        //    var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        //}
        //else if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext) 
        //{
        //    // ...
        //}

        if (context is null) throw new ArgumentNullException(nameof(AuthorizationHandlerContext));

        // DON'T ENGAGE for AllowAnonymousAttribute:
        if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
        {
            if (authorizationFilterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return Task.CompletedTask;
            }
        }

        if (context.Resource is HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            // DO NOT ENGAGE: If AllowAnonymousAttribute exists
            if (actionDescriptor != null && actionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return Task.CompletedTask;
            }
        }

        try
        {
            bool IsUserSessionValid = _httpContextAccessor?.HttpContext?.Session.ValidateUserSession() ?? false;


            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated && IsUserSessionValid)
            {
                context?.Succeed(requirement);
            }
            else
            {
                if (context.Resource is DefaultHttpContext defaultHttp)
                {
                    context.Succeed(requirement); // For redirecting to work !! 
                                                  // defaultHttp.Response.Redirect("~/Logout");
                    defaultHttp.Response.Redirect("/Account/Logout");

                }
                else if (context.Resource is AuthorizationFilterContext redirectContext)
                {
                    context.Succeed(requirement); // For redirecting to work !! 
                                                  // redirectContext.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                                                  //redirectContext.Result = new RedirectResult("~/Logout");
                    redirectContext.Result = new RedirectResult("/Account/Logout");

                }
                else
                {
                    // TODO IMPLEMENT: Check if context is stg else...
                    Type t = context.Resource.GetType();
                    context?.Fail();
                }
            }

            return Task.CompletedTask;
        }
        catch
        {
            throw;
        }






/*         try
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            ISession? _session = _httpContextAccessor?.HttpContext?.Session;

            if (_session != null)
            {
                if (_session.GetKey<bool>(Literals.SessionKey_Login))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }


            return Task.CompletedTask;
        }
        catch
        {
            throw;
        } */
    }
}
