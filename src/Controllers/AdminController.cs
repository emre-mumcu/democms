using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Attributes;
using src.App_Lib.Configuration.Ext;

namespace src.Controllers
{
    [Authorize(Policy = nameof(AuthorizationPolicyLibrary.adminPolicy))]
    [MenuItem]
    public class AdminController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
