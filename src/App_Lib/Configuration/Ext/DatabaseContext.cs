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
		});

		return services;
	}
}