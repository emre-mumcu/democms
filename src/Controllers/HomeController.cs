using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using src.App_Lib.Attributes;
using src.App_Lib.Cache;
using src.App_Lib.Configuration.Ext;

namespace src.Controllers
{
    [Authorize(Policy = nameof(AuthorizationPolicyLibrary.userPolicy))]
    [MenuItem]
    public class HomeController : Controller
    {
        public ActionResult Index([FromServices] IMemoryCache cache)
        {
            var states = new _CacheStates(cache).GetData();


            
            return View();
        }

       



    }
}
