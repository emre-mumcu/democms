using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using src.App_Data.Entities;
using src.App_Data.Types;
using src.App_Lib.Cache;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Extensions;
using src.App_Lib.Filters;

namespace src.App_Lib.Tools
{
    // Requires:
    // builder.Services.AddHttpContextAccessor();
    // builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

    public class ManagerTools
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IActionContextAccessor? _actionContextAccessor;
        private readonly LinkGenerator _generator;

        public ManagerTools(IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, LinkGenerator generator)
        {
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _generator = generator;
        }

        public string GetActionDescriptor()
        {
            return _actionContextAccessor?.ActionContext?.ActionDescriptor.Id ?? string.Empty;
        }

        public string GetControllerName()
        {
            if (_actionContextAccessor?.ActionContext is ControllerContext)
            {
                ControllerContext? controllerContext = _actionContextAccessor?.ActionContext as ControllerContext;
                return controllerContext?.ActionDescriptor.ControllerName ?? string.Empty;
            }

            return string.Empty;
        }

        public string GetActionName()
        {
            if (_actionContextAccessor?.ActionContext is ControllerContext)
            {
                ControllerContext? controllerContext = _actionContextAccessor?.ActionContext as ControllerContext;
                return controllerContext?.ActionDescriptor.ActionName ?? string.Empty;
            }

            return string.Empty;
        }

        public string GetCurrentUri()
        {
            var routeValues = _actionContextAccessor?.ActionContext?.ActionDescriptor.RouteValues;
            var scheme = _httpContextAccessor?.HttpContext?.Request.Scheme;
            HostString host = _httpContextAccessor?.HttpContext?.Request.Host ?? new HostString();

            return _generator.GetUriByAction(
                        action: GetActionName(),
                        controller: GetControllerName(),
                        values: routeValues,
                        scheme: scheme,
                        host: host)
                    ?? string.Empty;
        }

        public bool IsAdmin()
        {
            //return (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).HasRole(RoleEnum.SGKWEB_YAZILIM_GELISTIRICI.ToString());
            return (Security.Decrypt(_httpContextAccessor.HttpContext.Session.GetKey<string>(Literals.SessionKey_SelectedRole), _httpContextAccessor.HttpContext.Session.Id)).Equals(EnumRoles.ADMINISTRATOR.ToString());
        }



        public string GetUserIdentifier()
        {
            return (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetNameIdentifier();
        }






        public string GetUserNameSurname()
        {
            return $"{(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetName()} {(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetSurname()}";
        }



        public bool IsControllerAllowed(string controllerName)
        {
            return IsResourceAllowed(controllerName, "*");
        }

        public bool IsActionAllowed(string controllerName, string actionName)
        {
            return IsResourceAllowed(controllerName, actionName);
        }

        private bool IsResourceAllowed(string controllerName, string actionName)
        {
            string sessionId = _httpContextAccessor.HttpContext.Session.Id;
            string encryptedSelectedRole = _httpContextAccessor.HttpContext.Session.GetKey<string>(Literals.SessionKey_SelectedRole);
            string selectedRole = Security.Decrypt(encryptedSelectedRole, sessionId);

            List<RoleMatrix> RolCodeIcinVeriTabanindakiYetkiTanimlari = StartupCache.GetRoleMatrix(roleCode: selectedRole).Result;

            bool RedEdildi = DynamicRoleRequirementFilter.RedleriKontrolEt(
                redListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == false).ToList(),
                ErisilmekIstenenKaynak: $"{controllerName}.{actionName}",
                typeName: nameof(Controller));
            if (RedEdildi)
            {
                return false;
            }

            bool IzinVerildi = DynamicRoleRequirementFilter.IzinleriKontrolEt(
                izinListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == true).ToList(),
                ErisilmekIstenenKaynak: $"{controllerName}.{actionName}",
                typeName: nameof(Controller));
            if (!IzinVerildi)
            {
                return false;
            }

            return true;
        }

    }

}

/*
 *  Razor Syntax
 *  ------------
 * 	var controller = ViewContext.RouteData.Values["Controller"]?.ToString()?.ToLower(new System.Globalization.CultureInfo("en-us"));
 * 	var action = ViewContext.RouteData.Values["Action"]?.ToString()?.ToLower(new System.Globalization.CultureInfo("en-us"));
 * 	
 */