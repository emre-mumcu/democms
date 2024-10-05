using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;

namespace src.App_Lib.Configuration.Ext;

public static class Cookie
{
    public static IServiceCollection _AddCookieConfiguration(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
			// Require consent for non-essential cookies
			options.CheckConsentNeeded = context => true;			 
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        services.Configure(delegate (CookieTempDataProviderOptions options)
        {
            options.Cookie.IsEssential = true; // GDPR
		});

        return services;
    }
}