using System;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace src.App_Lib.Configuration.Ext;

public static class RequestLocalization
{
	public static IServiceCollection _AddRequestLocalization(this IServiceCollection services)
	{
		CultureInfo ciTR = new CultureInfo("tr-TR");
		CultureInfo ciEN = new CultureInfo("en-US");
		CultureInfo ciDE = new CultureInfo("de-DE");

		CultureInfo[] supportedCultures = new[]
		{
			ciTR, ciEN, ciDE
		};

		services.Configure<RequestLocalizationOptions>(options =>
		{
			options.DefaultRequestCulture = new RequestCulture(ciTR);
			options.SupportedCultures = supportedCultures;
			options.SupportedUICultures = supportedCultures;
			options.RequestCultureProviders = new List<IRequestCultureProvider>
			{
					new QueryStringRequestCultureProvider(),
					new CookieRequestCultureProvider()
			};

		});

		return services;
	}
}