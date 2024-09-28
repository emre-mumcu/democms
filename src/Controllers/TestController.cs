using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Attributes;
using src.App_Lib.Configuration.Ext;

namespace src.Controllers
{
    [Authorize(Policy = nameof(AuthorizationPolicyLibrary.userPolicy))]
    [MenuItem]
    public class TestController : Controller
    {
        [Authorize(Policy = nameof(AuthorizationPolicyLibrary.adminPolicy))]
        [MenuItem(_ParentText: "Home", _MenuText: "Banka Bilgileri", _ParentIndex: 10, _IsSingle: false, _ParentIconClass: "fa fa-cogs")]
        public ActionResult Index()
        {
            return View();
        }

    }
}
