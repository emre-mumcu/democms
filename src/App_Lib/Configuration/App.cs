namespace src.App_Lib.Configuration;

public sealed class App
{
	#region Singleton
	// https://csharpindepth.com/articles/singleton (Sixth version)

	private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

	public static App Instance { get => appInstance.Value; }

#pragma warning disable CS8618
	private App() {}
#pragma warning restore CS8618

	#endregion Singleton


	public IConfigurationRoot _DataConfiguration { get; set; }
	public IWebHostEnvironment _WebHostEnvironment { get => GetRequiredService<IWebHostEnvironment>(); }
	public IHttpContextAccessor _HttpContextAccessor { get => GetRequiredService<IHttpContextAccessor>(); }


	// Service Locator Pattern
	private IServiceProvider _ServiceProvider { get; set; }
	public void SetLocator(IServiceProvider serviceProvider) => _ServiceProvider = serviceProvider;
	public T? GetService<T>() => _ServiceProvider.GetService<T>();
	public T GetRequiredService<T>() where T : notnull => _ServiceProvider.GetRequiredService<T>();
}