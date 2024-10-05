using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Lib.Cache;
using src.App_Lib.Concrete;
using src.App_Lib.Configuration;

try
{
	App.Instance._DataConfiguration = new ConfigurationBuilder().AddJsonFile("data.json").Build();

	var builder = WebApplication.CreateBuilder(new WebApplicationOptions
	{
		ApplicationName = typeof(Program).Assembly.FullName,
		ContentRootPath = Directory.GetCurrentDirectory(),
		EnvironmentName = Environments.Development,
		WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
		Args = args
	})
	._ConfigureServices();

	var app = builder.Build()._Configure().Result;

	App.Instance.SetLocator(app.Services);	

	await DataSeeder.SeedData(app.Services);

	{
		// Static Configurations

		CacheKicker.Instance.Configure(app.Services.GetRequiredService<IMemoryCache>()).InitCachesAsync();
		MenuBuilder.Configure(app.Services.GetRequiredService<IAuthorizationPolicyProvider>());
	}

	app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")
{
	Host.CreateDefaultBuilder(args)
		.ConfigureServices(services => { services.AddMvc(); })
		.ConfigureWebHostDefaults(webBuilder =>
		{
			webBuilder.Configure((ctx, app) =>
			{
				app.Run(async (context) =>
				{
					await context.Response.WriteAsync($"Error in application: {ex.Message}");
				});
			});
		})
		.Build()
		.Run()
	;
}