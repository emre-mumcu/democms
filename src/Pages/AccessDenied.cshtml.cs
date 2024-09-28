using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace src.Pages
{
    [AllowAnonymous]
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

/*
public IActionResult AccessDenied()
        {
            bool IsUserSessionValid = HttpContext.Session.ValidateUserSession();

            if(!IsUserSessionValid)
            {
                return RedirectToAction("Logout");
            }

            return new JsonResult(
                new { StatusCode = StatusCodes.Status403Forbidden, StatusMessage = "Forbidden" }
            );
        }

public IActionResult LoginFail() => new JsonResult(
            new { StatusCode = StatusCodes.Status401Unauthorized, StatusMessage = "Unauthorized" }
        );        
*/
