using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using src.App_Lib.Cache;
using src.App_Lib.Configuration.Ext;

namespace src.App_Lib.Configuration;

public static class Configure
{
	public async static Task<WebApplication> _Configure(this WebApplication app)
	{
		var staticFilePath = app.Services.GetRequiredService<IOptions<DataOptions>>()?.Value?.Application?.CdnPath ?? "wwwroot";

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

		app.UseHttpsRedirection();

		app._UseSCP();

		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new PhysicalFileProvider(staticFilePath),
			RequestPath = "/cdn"
		});

		app._UseOptions();

		app._UseSession();

		app.UseRouting();

		app._UseAuthentication();

		app._UseAuthorization();

		app.UseHsts();

		app.MapDefaultControllerRoute();

		// app.MapControllers();

		app.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.MapRazorPages();

		// Keep
		await Task.FromResult(0);

		return app;
	}
}