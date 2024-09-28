using Microsoft.AspNetCore.Authorization;

namespace src.App_Lib.Requirements;

public class UserRequirement : IAuthorizationRequirement
{


    public string[] Roles { get; private set; }

    public UserRequirement(params string[] roles)
    {
        Roles = roles;
    }
}
