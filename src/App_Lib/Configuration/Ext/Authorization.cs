using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using src.App_Lib.Abstract;
using src.App_Lib.Concrete;
using src.App_Lib.Handlers;
using src.App_Lib.Requirements;

namespace src.App_Lib.Configuration.Ext;

public static class Authorization
{
    public static IServiceCollection _AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            /*
             * Gets or sets the default authorization policy with no policy name specified.
             * Defaults to require authenticated users.
             * 
             * The DefaultPolicy is the policy that is applied when:
             *      (*) You specify that authorization is required, either using RequireAuthorization(), by applying an AuthorizeFilter, 
             *          or by using the[Authorize] attribute on your actions / Razor Pages.
             *      (*) You don't specify which policy to use.
             *      
             */

            // options.DefaultPolicy = new AuthorizationPolicyBuilder()
            //     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .RequireAuthenticatedUser()
            //     .AddRequirements(new BaseRequirement())
            //     .Build();

            options.DefaultPolicy = AuthorizationPolicyLibrary.defaultPolicy;

            /*
             * Gets or sets the fallback authorization policy when no IAuthorizeData have been provided.
             * By default the fallback policy is null.
             * 
             * The FallbackPolicy is applied when the following is true:
             *      (*) The endpoint does not have any authorisation applied. No[Authorize] attribute, no RequireAuthorization, nothing.
             *      (*) The endpoint does not have an[AllowAnonymous] applied, either explicitly or using conventions.
             *      
             * So the FallbackPolicy only applies if you don't apply any other sort of authorization policy, 
             * including the DefaultPolicy, When that's true, the FallbackPolicy is used.
             * By default, the FallbackPolicy is a no - op; it allows all requests without authorization.
             * 
             */

            // options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .RequireAuthenticatedUser()
            //     .AddRequirements(new BaseRequirement())
            //     .Build();

            options.FallbackPolicy = AuthorizationPolicyLibrary.fallbackPolicy;

            options.InvokeHandlersAfterFailure = false;

            // Custom Policies:            
            options.AddPolicy(nameof(AuthorizationPolicyLibrary.userPolicy), AuthorizationPolicyLibrary.userPolicy);
            options.AddPolicy(nameof(AuthorizationPolicyLibrary.adminPolicy), AuthorizationPolicyLibrary.adminPolicy);
        });

        services.AddSingleton<IAuthorizationHandler, BaseHandler>();
        services.AddSingleton<IAuthorizationHandler, UserHandler>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        // TODO: Set Authorizer
        services.AddSingleton<IAuthorize, TestAuthorize>();

        return services;
    }

    public static IApplicationBuilder _UseAuthorization(this WebApplication app)
    {
        app.UseAuthorization();

        return app;
    }
}

public static class AuthorizationPolicyLibrary
{
    public static AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
       .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
       .RequireAuthenticatedUser()
       .AddRequirements(new BaseRequirement())
       .Build();

    public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
       .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
       .RequireAuthenticatedUser()
       .AddRequirements(new BaseRequirement())
       .Build();

    public static AuthorizationPolicy userPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .AddRequirements(new BaseRequirement())
        .AddRequirements(new UserRequirement("USER"))
        //.RequireRole("USER")
        .Build();

    public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .AddRequirements(new BaseRequirement())
        .AddRequirements(new UserRequirement("ADMIN", "ADMINISTRATOR"))
        //.RequireRole("ADMIN")
        .Build();

    //public static AuthorizationPolicy assertionPolicy = new AuthorizationPolicyBuilder()
    //    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
    //    .RequireAuthenticatedUser()
    //    .RequireRole("admin")
    //    // The Require Assertion method takes a lambda that receives the Http Context object and returns a Boolean value. 
    //    // Therefore, the assertion is simply a conditional statement.
    //    .RequireAssertion(ctx => { return ctx.User.HasClaim("editor", "contents") || ctx.User.HasClaim("level", "senior"); })
    //    .Build();
}

// https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authorization.Policy/PolicyEvaluator.cs
// https://github.com/dotnet/aspnetcore/blob/main/src/Shared/SecurityHelper/SecurityHelper.cs
// https://github.com/dotnet/aspnetcore/tree/main/src

public class MockPolicyEvaluator : IPolicyEvaluator
{
    private readonly IAuthorize _authorizer;
    private readonly IAuthorizationService _authorization;

    public MockPolicyEvaluator(IAuthorize authorizer, IAuthorizationService authorization)
    {
        _authorizer = authorizer;
        _authorization = authorization;
    }

    /// <summary>
    /// Does authentication for <see cref="AuthorizationPolicy.AuthenticationSchemes"/> and sets the resulting
    /// <see cref="ClaimsPrincipal"/> to <see cref="HttpContext.User"/>.  If no schemes are set, this is a no-op.
    /// </summary>
    /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns><see cref="AuthenticateResult.Success"/> unless all schemes specified by <see cref="AuthorizationPolicy.AuthenticationSchemes"/> failed to authenticate.
    public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        //if (policy.AuthenticationSchemes != null && policy.AuthenticationSchemes.Count > 0)
        //{
        //    ClaimsPrincipal? newPrincipal = null;

        //    foreach (var scheme in policy.AuthenticationSchemes)
        //    {
        //        var result = await context.AuthenticateAsync(scheme);

        //        if (result != null && result.Succeeded)
        //        {
        //            newPrincipal = MergeUserPrincipal(newPrincipal, result.Principal);
        //        }
        //    }

        //    if (newPrincipal != null)
        //    {
        //        context.User = newPrincipal;
        //        return AuthenticateResult.Success(new AuthenticationTicket(newPrincipal, string.Join(";", policy.AuthenticationSchemes)));
        //    }
        //    else
        //    {
        //        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        //        return AuthenticateResult.NoResult();
        //    }
        //}

        try
        {
            // If user requested the Account controller, do NOT engage
            if (context.Request.Path.Value != null && context.Request.Path.Value.StartsWith("/Account"))
                return await Task.FromResult(AuthenticateResult.NoResult());

            // TODO Username
            string username = "emumcu2";

            AuthenticationTicket ticket = _authorizer.GetTicket(username);
            context.User = ticket.Principal; // Set User

            context.Session.SetKey(Literals.SessionKey_Login, true);
            context.Session.SetKey(Literals.SessionKey_LoginUser, username);

            // Return success
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            //context.Response.Redirect("/Account/Login");
            return await Task.FromResult(AuthenticateResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Attempts authorization for a policy using <see cref="IAuthorizationService"/>.
    /// </summary>
    /// <param name="policy">The <see cref="AuthorizationPolicy"/>.</param>
    /// <param name="authenticationResult">The result of a call to <see cref="AuthenticateAsync(AuthorizationPolicy, HttpContext)"/>.</param>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="resource">
    /// An optional resource the policy should be checked with.
    /// If a resource is not required for policy evaluation you may pass null as the value.
    /// </param>
    /// <returns>Returns <see cref="PolicyAuthorizationResult.Success"/> if authorization succeeds.
    /// Otherwise returns <see cref="PolicyAuthorizationResult.Forbid"/> if <see cref="AuthenticateResult.Succeeded"/>, otherwise
    /// returns  <see cref="PolicyAuthorizationResult.Challenge"/></returns>
    public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        if (policy == null)
            throw new ArgumentNullException(nameof(policy));

        var result = await _authorization.AuthorizeAsync(context.User, resource, policy);

        if (result.Succeeded)
            return PolicyAuthorizationResult.Success();

        // If authentication was successful, return forbidden, otherwise challenge
        return authenticationResult.Succeeded ? PolicyAuthorizationResult.Forbid() : PolicyAuthorizationResult.Challenge();
    }

    /// <summary>
    /// Add all ClaimsIdentities from an additional ClaimPrincipal to the ClaimsPrincipal
    /// Merges a new claims principal, placing all new identities first, and eliminating
    /// any empty unauthenticated identities from context.User
    /// </summary>
    /// <param name="existingPrincipal">The <see cref="ClaimsPrincipal"/> containing existing <see cref="ClaimsIdentity"/>.</param>
    /// <param name="additionalPrincipal">The <see cref="ClaimsPrincipal"/> containing <see cref="ClaimsIdentity"/> to be added.</param>
    public static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal? existingPrincipal, ClaimsPrincipal? additionalPrincipal)
    {
        // For the first principal, just use the new principal rather than copying it
        if (existingPrincipal == null && additionalPrincipal != null)
        {
            return additionalPrincipal;
        }

        var newPrincipal = new ClaimsPrincipal();

        // New principal identities go first
        if (additionalPrincipal != null)
        {
            newPrincipal.AddIdentities(additionalPrincipal.Identities);
        }

        // Then add any existing non empty or authenticated identities
        if (existingPrincipal != null)
        {
            newPrincipal.AddIdentities(existingPrincipal.Identities.Where(i => i.IsAuthenticated || i.Claims.Any()));
        }
        return newPrincipal;
    }

}
