namespace src.App_Lib.Configuration.Ext;

public static class Options
{
	public static IServiceCollection _AddOptions(this IServiceCollection services, WebApplicationBuilder web_builder)
	{
		services.AddOptions();

		services.Configure<DataOptions>(web_builder.Configuration);

		return services;
	}
}

public partial class DataOptions
{
	public Application? Application { get; set; }
	public Database? Database { get; set; }
	public TokenParameters? TokenParameters { get; set; }
}

public partial class Application
{
	public string? Name { get; set; }
	public string? Version { get; set; }
	public string? CdnPath { get; set; }
	public string? GalleryPath { get; set; }
}

public partial class Database
{
	public string? ConnectionString { get; set; }
}

public partial class TokenParameters
{
	public string? Issuer { get; set; }
	public string? Audience { get; set; }
	public long Timeout { get; set; }
	public string? SignKey { get; set; }
	public string? EncryptKey { get; set; }
	public long ClockSkew { get; set; }
}