using System;
using src.App_Data;
using Microsoft.EntityFrameworkCore;

namespace src.App_Lib.Configuration.Ext;

public static class DatabaseContext
{
	public static IServiceCollection _AddDbContext(this IServiceCollection services)
	{
		services.AddDbContext<AppDbContext>((serviceProvider, options) =>
		{
			options.UseSqlite(App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value);
			//options.UseInternalServiceProvider(serviceProvider);
		});



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


		return services;
	}

	public static IApplicationBuilder _UseDbContext(this WebApplication app)
	{
		return app;
	}
}