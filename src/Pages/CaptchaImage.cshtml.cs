using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.App_Lib.Tools;

namespace src.Pages
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class CaptchaImageModel : PageModel
    {
        public IActionResult OnGet()
        {
            var result = Captcha3.GenerateCaptchaImage(HttpContext);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
    }
}
