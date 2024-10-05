using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using src.App_Lib.Abstract;
using src.App_Lib.Concrete;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Filters;
using src.App_Lib.Tools;

namespace src.App_Lib.Configuration;

public static class ConfigureServices
{
	public static WebApplicationBuilder _ConfigureServices(this WebApplicationBuilder web_builder)
	{
		web_builder.Configuration.AddJsonFile($"data.json", optional: false, reloadOnChange: false);

		web_builder.Services._AddRequestLocalization();

		IMvcBuilder mvcBuilder = web_builder.Services.AddMvc(config =>
		{
			config.Filters.Add(new AuthorizeFilter());
			config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
		})
		// Use session based TempData instead of cookie based TempData
		.AddSessionStateTempDataProvider();

		mvcBuilder._AddJsonOptions();

		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
			mvcBuilder.AddRazorRuntimeCompilation();    
		}

		web_builder.Services.AddHttpContextAccessor();

		web_builder.Services.AddDataProtection();

		web_builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

		web_builder.Services._AddCookieConfiguration();

		web_builder.Services._AddSession();

		web_builder.Services._AddOptions(web_builder);

		web_builder.Services._AddAuthentication();

		web_builder.Services._AddAuthorization();

		web_builder.Services._ConfigureViewLocationExpander();

		// TODO: MockPolicyEvaluator
		// #if DEBUG
		// web_builder.Services.AddSingleton<IPolicyEvaluator, MockPolicyEvaluator>();
		// #endif

		web_builder.Services.AddScoped<IActionLogService, ActionLogService>();

		web_builder.Services.AddScoped<ITokenService, TokenService>();

		web_builder.Services.AddTransient<IAppVersionService, AppVersionService>();

		web_builder.Services.AddScoped<ManagerTools>();

		web_builder.Services.AddScoped<WebExceptionFilter>();

		web_builder.Services.AddScoped<GlobalExceptionFilter>();

		web_builder.Services._AddDbContext();

		return web_builder;
	}
}