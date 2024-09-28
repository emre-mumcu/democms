using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using src.App_Lib.Abstract;
using static src.App_Lib.Tools.Captcha3;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Configuration;
using src.App_Data.Types;
using src.App_Lib.Tools;

namespace src.Pages
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public partial class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel loginViewModel { get; set; } = new LoginViewModel();


        private readonly ILogger<LoginModel> _logger;
        private readonly IAuthenticate _authenticator;
        private readonly IAuthorize _authorizer;

        public LoginModel(ILogger<LoginModel> logger, IAuthenticate authenticator, IAuthorize authorizer)
        {
            _logger = logger;
            _authenticator = authenticator;
            _authorizer = authorizer;
        }

        public async Task<IActionResult> OnGet([FromServices] IOptions<DataOptions> options)
        {
            // TODO !!! AutoLogin
            // _logger.LogInformation(1, $"User {Environment.UserName} has been logged");
//#if DEBUG
//            return await LoginUser(Environment.UserName, "", true, "");
//#endif

            // Cache Nas�l Temizlenir:
            // CacheReset cacheReset = new CacheReset(options);
            // var sonuc = cacheReset.ResetCache(SGKWebSayfasi.Entities.ComplexTypes.CacheNameEnum.AnnouncementCache.GetEnumDescription());

            if (User?.Identity?.IsAuthenticated ?? false) return RedirectToAction("Index", "Home");
            else return Page();
        }

        public async Task<IActionResult> OnPost([Bind(Prefix = "loginViewModel")] LoginViewModel model, string? returnurl)
        {
            void ClearCaptchaText()
            {
                // Even the CaptchaCode is cleared in Model,
                // without clearing the Captcha.CaptchaCode in ModelState ,
                // CaptchaCode textbox is preserving the previous entry !!!
                model.Captcha.CaptchaCode = string.Empty;
                // ModelState.SetModelValue("Captcha.CaptchaCode", new ValueProviderResult(string.Empty)); // Not working!!!
                ModelState.Remove("Captcha.CaptchaCode");
            }

            if (!ModelState.IsValid)
            {
                ClearCaptchaText();
                ModelState.AddModelError("ERR", $"Formda hatalar var. Lütfen hataları düzeltip, işleminizi yeniden deneyiniz.");
                return Page();
            }
            else
            {
                try
                {
                    if (!Captcha3.ValidateCaptchaCode(model.Captcha.CaptchaCode, HttpContext))
                    {
                        ClearCaptchaText();
                        ModelState.AddModelError("Captcha", "Güvenlik kodu yanlış.");
                        return Page();
                    }
                    else
                    {
                        return await LoginUser(model.Username, model.Password, model.RememberMe, returnurl);
                    }




                }
                catch (Exception ex)
                {
                    ClearCaptchaText();
                    ModelState.AddModelError("EX", ex.Message);
                    return Page();
                }
            }
        }
    }

    public partial class LoginModel
    {
        [NonAction]
        private async Task<IActionResult> LoginUser(string username, string password, bool remember, string? ReturnUrl)
        {
            if (_authenticator.AuthenticateUser(username, password))
            {
                AuthenticationTicket ticket = _authorizer.GetTicket(username);

                HttpContext.Session.SetKey<bool>(Literals.SessionKey_Login, true);
                HttpContext.Session.SetKey<string>(Literals.SessionKey_LoginUser, username);

                
                // Authorizer gelen roller ile RoleEnum farkli olur ise RoleEnum esas alınacak
                List<EnumRoles> userRoles = ticket.Principal.Claims
                    .Where(c => c.Type == ClaimTypes.Role && Enum.TryParse<EnumRoles>(c.Value, out _))
                    .Select(i => Enum.Parse<EnumRoles>(i.Value)).ToList();

                if (!(userRoles.Count() > 0))  throw new Exception("Uygulamayı kullanmak için yetkiniz bulunmamaktadır.");

                // En yüksek seviyeli rolü seç
                EnumRoles highest = userRoles.OrderByDescending(i => (int)i).First();
                HttpContext.Session.SetKey(Literals.SessionKey_SelectedRole, Security.Encrypt(highest.ToString(), HttpContext.Session.Id));

                HttpContext.Session.CreateUserSession(ticket.Principal);

                await HttpContext.SignInAsync(
                    ticket.AuthenticationScheme,
                    ticket.Principal,
                    ticket.Properties
                );

                if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != "/Logout")
                    return LocalRedirect(ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                throw new Exception("Kullanıcı bilgileri hatalı.");
            }
        }
    }

    public class LoginViewModel
    {
        // {0} display name of property,
        // {1} is the MaximumLength,
        // {2} is the MinimumLength
        //[Required(ErrorMessage = "{0} is required")]
        [Required(ErrorMessage ="{0} alanı zorunludur")]
        [Display(Name ="Kullanıcı Adınız")]
        [StringLength(255, ErrorMessage = "{0} must be between {2} and {1} characters", MinimumLength = 5)]        
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} alanı zorunludur")]
        [Display(Name = "Parolanız")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        //[Compare("Password")]
        //public string? ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur")]
        [Display(Name = "Güvenlik Kodu")]
        public CaptchaResult Captcha { get; set; } = new CaptchaResult();
    }



}
