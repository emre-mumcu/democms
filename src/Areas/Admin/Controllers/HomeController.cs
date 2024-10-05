using Microsoft.AspNetCore.Mvc;

namespace src.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : Controller
	{
		public async Task<ActionResult> Index() => await Task.Run(() => View());
	}
}