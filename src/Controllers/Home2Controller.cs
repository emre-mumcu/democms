using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Attributes;

namespace src.Controllers
{
    [DynamicRoleRequirement(typeof(Home2Controller))]
    public class Home2Controller : Controller
    {
        // GET: Home2Controller
        public ActionResult Index()
        {
            return View();
        }

        [DynamicRoleRequirement(typeof(HomeController), nameof(Deneme))]
        public async Task<IActionResult> Deneme() => await Task.Run(() => Content("Deneme"));

    }
}
