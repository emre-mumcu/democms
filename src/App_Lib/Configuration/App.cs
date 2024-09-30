namespace src.App_Lib.Configuration;

public sealed class App
{
	// https://csharpindepth.com/articles/singleton (Sixth version)

	private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

	public static App Instance { get { return appInstance.Value; } }

	private App()
	{
		// _DataConfiguration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("data.json", true).Build();
	}

	public IConfigurationRoot _DataConfiguration { get; set; }
	public IWebHostEnvironment _WebHostEnvironment { get; set; }
	public IHttpContextAccessor _HttpContextAccessor { get; set; }
	public IServiceProvider _ServiceProvider { get; set; }
}