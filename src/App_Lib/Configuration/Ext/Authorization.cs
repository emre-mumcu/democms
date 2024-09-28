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
		.AddRequirements(new UserRequirement("ADMINISTRATOR", "USER"))
		//.RequireRole("USER")
		.Build();

	public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
		.RequireAuthenticatedUser()
		.AddRequirements(new BaseRequirement())
		.AddRequirements(new UserRequirement("ADMINISTRATOR"))
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