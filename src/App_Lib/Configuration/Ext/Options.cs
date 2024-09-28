using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.App_Lib.Configuration.Ext
{
    public static class Options
    {
        public static IServiceCollection _AddOptions(this IServiceCollection services, WebApplicationBuilder web_builder)
        {
            services.AddOptions();

            // IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("data.json", optional: true, reloadOnChange: true);
            // IConfigurationRoot ConfigurationRoot = builder.Build();
            // services.Configure<DataOptions>(ConfigurationRoot.GetSection("Application"));
            // services.Configure<DataOptions>(web_builder.Configuration.GetSection("Application"));

            services.Configure<DataOptions>(web_builder.Configuration);

            return services;
        }

        public static IApplicationBuilder _UseOptions(this WebApplication app)
        {
            return app;
        }
    }

    public partial class DataOptions
    {
        public Application? Application { get; set; }
        public Database? Database { get; set; }
    }

    public partial class Application
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? CdnPath { get; set; }
        public string? CdnPath2 { get; set; }
        public string? GalleryPath { get; set; }
    }

    public partial class Database
    {
        public string? ConnectionString { get; set; }
    }
}