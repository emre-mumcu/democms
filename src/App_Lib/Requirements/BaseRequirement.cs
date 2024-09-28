using Microsoft.AspNetCore.Authorization;

namespace src.App_Lib.Requirements;

public class BaseRequirement : IAuthorizationRequirement
{
    public BaseRequirement() { }
}