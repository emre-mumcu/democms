using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Attributes;

namespace src.Controllers
{
	[DynamicRoleRequirement(typeof(HomeController))]
	public class DynamicRoleController : Controller
    {
		[DynamicRoleRequirement(typeof(HomeController), nameof(Index))]
		public async Task<IActionResult> Index() => await Task.Run(() => Content("Deneme"));

	}
}