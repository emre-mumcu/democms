
namespace src.App_Lib.Configuration.Ext;

public static class StatusCodePages
{
    public static IServiceCollection _AddSCP(this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder _UseSCP(this WebApplication app)
    {

        //app.UseStatusCodePagesWithReExecute("/StatusResult/{0}");
        //app.UseStatusCodePages();

        app.UseStatusCodePagesWithReExecute("/StatusResult", "?statusCode={0}");
        return app;
    }
}