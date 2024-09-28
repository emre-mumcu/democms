using System.Reflection;
using src.App_Lib.Abstract;

namespace src.App_Lib.Concrete;

public class AppVersionService : IAppVersionService
{
	public string Version =>
		Assembly.GetEntryAssembly()
			?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
			?.InformationalVersion
			?? "0.0.0.0";
}