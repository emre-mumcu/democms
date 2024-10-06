
namespace src.App_Lib.Configuration.Ext;

public static class StatusCodePages
{
    public static IApplicationBuilder _UseSCP(this WebApplication app)
    {
		// app.UseStatusCodePages();
		// app.UseStatusCodePagesWithReExecute("/StatusResult/{0}");
		app.UseStatusCodePagesWithReExecute("/StatusResult", "?statusCode={0}");
		
        return app;
    }
}