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

# DbContext Registration

```cs

// In this type of service registration, connection string is NOT provided to DI.
// It must be provided in DbContext's OnConfiguring method.
builder.Services.AddDbContext<AppDbContext>();

// In this type of service registration, connection string is provided to DI.
// If DbContext is created by DI, connection string is present in the instance.
// But if user manually creates DbContext, the connection string must also be provided in DbContext's OnConfiguring method
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString: ""));

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
	options.UseInternalServiceProvider(serviceProvider);
	options.UseSqlite(App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value);
});

builder.Services.AddDbContext<AppDbContext>((provider, options) => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
	options.UseSqlite(dataConfiguration.GetSection("Database:ConnectionString").Value);
	options.UseInternalServiceProvider(serviceProvider);
});

// Manually            
builder.Services.AddScoped(x => { return new AppDbContext(); });
```

# IConfigurationBuilder

```cs
IConfigurationBuilder builder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("data.json", true)
	.Build()
; 

IConfigurationBuilder builder = new ConfigurationBuilder()
	.AddJsonFile("data.json", optional: true, reloadOnChange: true)
;
```

# Options Pattern

```cs
services.Configure<DataOptions>(web_builder.Configuration);
services.Configure<DataOptions>(ConfigurationRoot.GetSection("Application"));
services.Configure<DataOptions>(web_builder.Configuration.GetSection("Application"));
```