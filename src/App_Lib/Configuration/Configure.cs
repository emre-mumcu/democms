using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using src.App_Lib.Cache;
using src.App_Lib.Configuration.Ext;
using System.Diagnostics;

namespace src.App_Lib.Configuration;

public static class Configure
{
    public async static Task<WebApplication> _Configure(this WebApplication app)
    {


		app._InitApp();
        app.UseHttpsRedirection();
        app._UseSCP();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(app.Services.GetRequiredService<IOptions<DataOptions>>().Value.Application.CdnPath),
            RequestPath = "/cdn"
        });
        app._UseOptions();
        app._UseSession();
        app.UseRouting();
       app._UseAuthentication();
        app._UseAuthorization();
        app.UseHsts();
        // Keep
        await Task.FromResult(0);

        StartupCache.StartupCacheConfig(app.Services.GetRequiredService<IMemoryCache>());

        app.MapDefaultControllerRoute();

        app.MapControllers();

        app.MapRazorPages();

        return app;
    }
}