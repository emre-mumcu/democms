using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using src.App_Lib.Abstract;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;

namespace src.App_Lib.Evaluators;

/// <summary>
/// This class requires review!!!
/// </summary>
public class MockPolicyEvaluator : IPolicyEvaluator
{
	// TODO: Review is required!!!
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

		/*

		if (!context.Request.Headers.ContainsKey("Authorization"))
					return Task.FromResult(AuthenticateResult.Fail("No Authorization header found!"));

				string authHeader = context.Request.Headers["Authorization"];

				string bearerToken = authHeader?.Replace("Bearer ", string.Empty);

				if (!string.Equals(bearerToken, "authToken", StringComparison.Ordinal))
				{
					return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
				}
		*/


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

		/*
		// Really checks the Authorization by calling the IAuthorizationService
        var authorizationService = context.RequestServices.GetService<IAuthorizationService>();
        var result = await authorizationService.AuthorizeAsync(authenticationResult.Principal, policy);
        return result.Succeeded ? PolicyAuthorizationResult.Success() : PolicyAuthorizationResult.Forbid();

    
		*/

		/*
		var authorizeResult = authenticationResult.Succeeded
			? PolicyAuthorizationResult.Success()
			: PolicyAuthorizationResult.Challenge();

		return Task.FromResult(authorizeResult);
		*/
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

/*
public sealed class BearerAuthorizeFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context?.HttpContext?.Request?.Headers == null) throw new ArgumentNullException(nameof(context));

        if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            context.Result = CreateUnauthorized();

        var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
        var authenticateResult = await policyEvaluator.AuthenticateAsync(default, context.HttpContext);
        var authorizeResult = await policyEvaluator.AuthorizeAsync(default, authenticateResult, context.HttpContext, context);

        if (authorizeResult.Challenged)
        {
            context.Result = CreateUnauthorized();
            return;
        }

        context.HttpContext.User = authenticateResult.Principal;

        static IActionResult CreateUnauthorized() => new UnauthorizedObjectResult(new ErrorMessage("Unauthorized", 401));
    }
}
public void ConfigureServices(IServiceCollection services)
{
   services.AddControllers(o => o.Filters.Add(new BearerAuthorizeFilter()));
}
*/