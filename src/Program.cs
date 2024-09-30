using Microsoft.AspNetCore.Authorization;
using src.App_Data;
using src.App_Lib.Concrete;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;

try
{
    // var builder = WebApplication.CreateBuilder(args);

	var dataConfiguration = new ConfigurationBuilder().AddJsonFile("data.json").Build();

	App.Instance._DataConfiguration = dataConfiguration; // Keep this at the top!

	var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environments.Development,
        WebRootPath = dataConfiguration.GetSection("Application").GetSection("CDNPath").Value,
        Args = args
    })
    ._ConfigureServices();

	var app = builder.Build()._Configure().Result;

    App.Instance._WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
    App.Instance._HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
	App.Instance._ServiceProvider = app.Services;

    await DataSeeder.SeedData(app.Services);

    MenuBuilder.Configure(app.Services.GetRequiredService<IAuthorizationPolicyProvider>());

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