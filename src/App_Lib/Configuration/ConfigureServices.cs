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

		web_builder.Services._InitMVC();

		web_builder.Services._AddCookieConfiguration();

		web_builder.Services._AddSession();

		web_builder.Services._AddOptions(web_builder);

		web_builder.Services._AddAuthentication();

		web_builder.Services._AddAuthorization();

		web_builder.Services._ConfigureViewLocationExpander();

		// TODO: MockPolicyEvaluator
		//#if DEBUG
		//            // TODO MockPolicyEvaluator !!!
		//            web_builder.Services.AddSingleton<IPolicyEvaluator, MockPolicyEvaluator>();
		//#endif

		web_builder.Services.AddScoped<IActionLogService, ActionLogService>();

		web_builder.Services.AddScoped<ITokenService, TokenService>();

		web_builder.Services.AddTransient<IAppVersionService, AppVersionService>();

		web_builder.Services.AddScoped<ManagerTools>();

		// TODO:  Hata Yönetimi
		web_builder.Services.AddScoped<WebExceptionFilter>();

		web_builder.Services.AddScoped<GlobalExceptionFilter>();


		// TODO: AppDbContext
		web_builder.Services._AddDbContext();

		return web_builder;
	}
}