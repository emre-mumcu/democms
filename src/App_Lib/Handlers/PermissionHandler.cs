using Microsoft.AspNetCore.Authorization;

namespace src.App_Lib.Handlers;

public class PermissionHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            // validate each requirement
        }

        return Task.CompletedTask;
    }
}
