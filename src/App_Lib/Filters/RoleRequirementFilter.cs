using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.App_Data.Types;

namespace src.App_Lib.Filters;

public class RoleRequirementFilter : IAuthorizationFilter
{
    private readonly EnumRoles[] allowedRoles;

    public RoleRequirementFilter(EnumRoles[] AllowedRoles)
    {
        allowedRoles = AllowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool isAuthorized = false;

        var user = context.HttpContext.User;

        foreach (EnumRoles s in allowedRoles)
        {
            if (user.HasClaim(ClaimTypes.Role, s.ToString()))
            {
                isAuthorized = true;
            }
        }

        if (!isAuthorized) context.Result = new ForbidResult();
    }
}