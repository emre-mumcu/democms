using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace src.App_Lib.Configuration.Ext;

public static class Init
{
    public static IServiceCollection _InitMVC(this IServiceCollection services)
    {


        {
			// RequestLocalizationOptions

			CultureInfo ciTR = new CultureInfo("tr-TR");
            CultureInfo ciEN = new CultureInfo("en-US");
            CultureInfo ciDE = new CultureInfo("de-DE");

            CultureInfo[] supportedCultures = new[] { ciTR, ciEN, ciDE };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(ciTR);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };

            });
        }



        IMvcBuilder mvcBuilder = services.AddMvc(config =>
        {
            config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        })
        .AddSessionStateTempDataProvider(); // Use session based TempData instead of cookie based TempData


        {
            // Newtonsoft.Json Configuration

            mvcBuilder.AddNewtonsoftJson(options => // dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            });
        }


        {
            // System.Text.Json Configuration

            mvcBuilder.AddJsonOptions(options => {
                options.JsonSerializerOptions.WriteIndented = true;
                // Default is JsonNamingPolicy.CamelCase. 
                // Setting it to null will result in property names NOT changing while serializing
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        }
        
        
        services.AddHttpContextAccessor();


        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


        // IServiceProvider serviceProvider = services.BuildServiceProvider();
        // IWebHostEnvironment environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();


        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {

            mvcBuilder.AddRazorRuntimeCompilation(); // dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation   
        }


        services.AddDataProtection();


        return services;
    }

    public static IApplicationBuilder _InitApp(this WebApplication app)
    {

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }


        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseRequestLocalization();

        return app;
    }
}