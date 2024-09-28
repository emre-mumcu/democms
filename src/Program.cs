using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using src.App_Data;
using src.App_Lib.Concrete;
using src.App_Lib.Configuration;
using System.Diagnostics;

try
{
    // var builder = WebApplication.CreateBuilder(args);

    //Debugger.Launch();


	var dataConfiguration = new ConfigurationBuilder().AddJsonFile("data.json").Build();

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environments.Development,
        WebRootPath = dataConfiguration.GetSection("Application").GetSection("CDNPath").Value,
        Args = args
    })
    ._ConfigureServices();


    builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) => {
        options.UseSqlite(dataConfiguration.GetSection("Database:ConnectionString").Value);
        //options.UseInternalServiceProvider(serviceProvider);
    });

    // builder.Configuration.AddJsonFile($"data.json", optional: true, reloadOnChange: false);

    // builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    //builder.Services.AddDbContext<AppDbContext>((provider, options) => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    /*     builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseSqlite(dataConfiguration.GetSection("Database:ConnectionString").Value);
            options.UseInternalServiceProvider(serviceProvider);
        }); */

    {   // DbContext:

        // In this type of service registration, connection string is NOT provided to DI.
        // It must be provided in DbContext's OnConfiguring method.
        //builder.Services.AddDbContext<AppDbContext>();

        // In this type of service registration, connection string is provided to DI.
        // If DbContext is created by DI, connection string is present in the instance.
        // But if user manually creates DbContext, the connection string must also be provided in DbContext's OnConfiguring method
        // builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString: ""));

        // Manually            
        // builder.Services.AddScoped(x => { return new AppDbContext(); });
    }

    // builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // builder.Services.AddDbContext<AppDbContext>();

    // Keep it here above the builder.Build();
    App.Instance._DataConfiguration = dataConfiguration;

	var app = builder.Build()._Configure().Result;

    App.Instance._WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
    App.Instance._HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

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