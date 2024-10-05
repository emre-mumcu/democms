# Configuration Extension Template

```cs
public static class ConfigName
{
	public static IServiceCollection _AddServiceName(this IServiceCollection services)
	{
		// IServiceProvider serviceProvider = services.BuildServiceProvider();
        // IWebHostEnvironment environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
		return services;
	}

	public static IApplicationBuilder _UseServiceName(this WebApplication app)
	{
		return app;
	}
}
```