using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;

namespace src.App_Lib.Configuration.Ext;

public static class Cookie
{
    // Mükerrere Sessionda da vardı sanırım
    public static IServiceCollection _AddCookieConfiguration(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        services.Configure(delegate (CookieTempDataProviderOptions options)
        {
            options.Cookie.IsEssential = true;
        });

        return services;
    }

    public static IApplicationBuilder _UseCookie(this WebApplication app)
    {
        app.UseCookiePolicy();

        return app;
    }
}

// Mükerrere Sessionda da vardı sanırım
public class AppCookieAuthenticationEvents : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        // TODO: ValidatePrincipal implementation, runs in every request
        await Task.Delay(0);

        /*

        Boolean login = context.HttpContext.Session.GetKey<bool>(Literals.SessionKeyLogin);

        if (context.Principal != null && context.Principal.Identity != null)
        {
            if (!(context.Principal.Identity.IsAuthenticated && login))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
        else
        {
            // TODO: ?
        }

        */
    }
}
