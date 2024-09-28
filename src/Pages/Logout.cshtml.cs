using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using System.Security.Claims;

namespace src.Pages
{
    public partial class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            return await LogoutUser();
        }
    }

    public partial class LogoutModel
    {
        [NonAction]
        private async Task<RedirectResult> LogoutUser()
        {
            HttpContext.Session.RemoveKey(Literals.SessionKey_Login);
            HttpContext.Session.RemoveKey(Literals.SessionKey_LoginUser);

            HttpContext.User = new ClaimsPrincipal();

            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var session_cookie = HttpContext.Request.Cookies[Literals.Session_Cookie_Name];
            if (session_cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Response.Cookies.Append(Literals.Session_Cookie_Name, session_cookie, options);
            }

            var auth_cookie = HttpContext.Request.Cookies[Literals.Auth_Cookie_Name];
            if (auth_cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Response.Cookies.Append(Literals.Session_Cookie_Name, auth_cookie, options);
            }

            return new RedirectResult("/Login");
        }
    }
}
