using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using src.App_Lib.Abstract;
using src.App_Lib.Concrete;

namespace src.App_Lib.Configuration.Ext
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection _AddAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = Literals.Auth_Cookie_Name;
                    options.LoginPath = Literals.Auth_Cookie_LoginPath;
                    options.LogoutPath = Literals.Auth_Cookie_LogoutPath;
                    options.AccessDeniedPath = Literals.Auth_Cookie_AccessDeniedPath;
                    options.ClaimsIssuer = Literals.Auth_Cookie_ClaimsIssuer;
                    options.ReturnUrlParameter = Literals.Auth_Cookie_ReturnUrlParameter;
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true; // false: xss vulnerability !!!
                    options.ExpireTimeSpan = Literals.Auth_Cookie_ExpireTimeSpan;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Validate();
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                })
            ;

            services.AddScoped<CustomCookieAuthenticationEvents>();

            // TODO: Authenticator
            services.AddSingleton<IAuthenticate, TestAuthenticate>();

            return services;
        }

        public static IApplicationBuilder _UseAuthentication(this WebApplication app)
        {
            app.UseAuthentication();

            return app;
        }
    }

    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            // TODO: runs in every request

            bool login = context.HttpContext.Session.GetKey<bool>(Literals.SessionKey_Login);

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
        }
    }
}