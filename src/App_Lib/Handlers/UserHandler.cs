using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using src.App_Lib.Concrete;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Requirements;

namespace src.App_Lib.Handlers;

public class UserHandler : AuthorizationHandler<UserRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
    {

        //context.Succeed(requirement);
        //return Task.CompletedTask;

        var c = context;


        try
        {


            if (context.Resource is HttpContext httpContext)
            {
                var endpoint = httpContext.GetEndpoint();
                var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
            }
            else if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext)
            {
                // ...
            }



            {

                AppUser? appUser = _httpContextAccessor.HttpContext?.Session.GetKey<AppUser>(Literals.SessionKey_AppUser);

                string[] UserRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

                // if (requirement.Roles.Contains(appUser?.SelectedRole))
                // Using Any() assures that the intersection algorithm stops when the first equal object is found.
                if (requirement.Roles.Intersect(UserRoles).Any())
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else
                {
                    // context.Fail(); redirects users to accessdenied                        
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
        }
        catch
        {
            throw;
        }

/*         try
        {
            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                string[] roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

                // Using Any() assures that the intersection algorithm stops when the first equal object is found.
                if (requirement.Roles.Intersect(roles).Any())
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            else
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
        catch
        {
            throw;
        } */
    }
}
