using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.App_Data.Entities;
using src.App_Lib.Cache;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Tools;

namespace src.App_Lib.Filters
{
    public class DynamicRoleRequirementFilter : IAuthorizationFilter
    {
        private readonly Type _type;
        private readonly string? _memberName;

        public DynamicRoleRequirementFilter(Type type, string? memberName)
        {
            _type = type;
            _memberName = memberName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // SgkWebContext dbContext = context.HttpContext.RequestServices.GetService(typeof(SgkWebContext)) as SgkWebContext;

            string ErisilmekIstenenKaynak_AdAlani = _type.Namespace;
            string ErisilmekIstenenKaynak_TamAdi = _type.FullName;
            string ErisilmekIstenenKaynak_UyeAdi = _type.Name;

            string userSelectedRoleEnc = context.HttpContext.Session.GetKey<string>(Literals.SessionKey_SelectedRole);
            string userSelectedRole = Security.Decrypt(userSelectedRoleEnc, context.HttpContext.Session.Id);

            List<UserRoleRight> RolCodeIcinVeriTabanindakiYetkiTanimlari = StartupCache.GetDbYetkiler(roleCode: userSelectedRole).Result;

            bool RedEdildi = RedleriKontrolEt(
                redListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == false).ToList(),
                ErisilmekIstenenKaynak: $"{ErisilmekIstenenKaynak_TamAdi}.{_memberName}",
                typeName: _type?.BaseType?.Name);
            if (RedEdildi)
            {
                context.Result = new ForbidResult();
                return;
            }

            bool IzinVerildi = IzinleriKontrolEt(
                izinListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == true).ToList(),
                ErisilmekIstenenKaynak: $"{ErisilmekIstenenKaynak_TamAdi}.{_memberName}",
                typeName: _type?.BaseType?.Name);
            if (!IzinVerildi)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        /// <summary>
        /// RED durumları İZİN lerden önce kontrol edilmelidir.
        /// typeName: nameof(Controller) or nameof(PageModel)
        /// </summary>
        public static bool RedleriKontrolEt(List<UserRoleRight> redListesi, string ErisilmekIstenenKaynak, string? typeName = nameof(Controller))
        {
            foreach (UserRoleRight yetki in redListesi)
            {
                if (typeName == nameof(PageModel))
                {
                    if (yetki.FullName == ErisilmekIstenenKaynak)
                    {
                        //red var
                        return true;
                    }
                }

                if (typeName == nameof(Controller))
                {
                    if (yetki.FullName == ErisilmekIstenenKaynak)
                    {
                        //red var
                        return true;
                    }
                }
            }

            // red yok
            return false;
        }

        /// <summary>
        /// RED durumları İZİN lerden önce kontrol edilmelidir.
        /// typeName: nameof(Controller) or nameof(PageModel)
        /// </summary>
        public static bool IzinleriKontrolEt(List<UserRoleRight> izinListesi, string ErisilmekIstenenKaynak, string? typeName = nameof(Controller))
        {
            foreach (UserRoleRight yetki in izinListesi)
            {
                if (typeName == nameof(PageModel))
                {
                    if (yetki.FullName == ErisilmekIstenenKaynak)
                    {
                        //izin var
                        return true;
                    }
                }

                if (typeName == nameof(Controller))
                {
                    if (yetki.FullName == ErisilmekIstenenKaynak)
                    {
                        //izin var
                        return true;
                    }
                }
            }

            // izin yok
            return false;
        }
    }

}

//StackTrace st = new StackTrace();
//StackFrame currentFrame = st.GetFrame(1);
//MethodBase method = currentFrame.GetMethod();