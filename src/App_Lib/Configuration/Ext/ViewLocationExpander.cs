using Microsoft.AspNetCore.Mvc.Razor;

namespace src.App_Lib.Configuration.Ext;

public static class ViewLocation
{
	public static IServiceCollection _ConfigureViewLocationExpander(this IServiceCollection services)
	{
		services.Configure<RazorViewEngineOptions>(options =>
		{
			options.ViewLocationExpanders.Add(new ViewLocationExpander());
		});

		return services;
	}
}

public class ViewLocationExpander : IViewLocationExpander
{
	public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
	{
		//{2} is area, {1} is controller,{0} is the action
		string[] locations = new string[] {
			"/Content/{0}" + RazorViewEngine.ViewExtension,			
			"/Content/PartialViews/{0}" + RazorViewEngine.ViewExtension,			
		};

		return locations.Union(viewLocations);
	}

	public void PopulateValues(ViewLocationExpanderContext context)
	{
		context.Values["customviewlocation"] = nameof(ViewLocationExpander);
	}
}