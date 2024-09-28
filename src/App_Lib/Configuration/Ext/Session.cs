using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using src.App_Data.Types;
using src.App_Lib.Concrete;

namespace src.App_Lib.Configuration.Ext
{
    public static class Session
    {
        public static IServiceCollection _AddSession(this IServiceCollection services)
        {
            // The IDistributedCache implementation is used as a backing store for session
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = Literals.Session_Cookie_Name;
                options.IdleTimeout = Literals.Session_IdleTimeout;
                options.Cookie.HttpOnly = true; // security!!!
                options.Cookie.IsEssential = true; // GDPR bypass!!!
                // options.Cookie.SameSite = SameSiteMode.Lax;
            });

            return services;
        }

        public static IApplicationBuilder _UseSession(this WebApplication app)
        {
            app.UseSession();

            return app;
        }
    }

    public static class SessionExtensions
    {
        public static void SetKey<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetKey<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool HasKey(this ISession session, string key)
        {
            return session.HasKey(key);
        }

        public static void RemoveKey(this ISession session, string key)
        {
            session.Remove(key);
        }

        public static void CreateUserSession(this ISession session, ClaimsPrincipal User)
        {
            try
            {
                AppUser appUser = new AppUser()
                {
                    UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value,
                    UserRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList(),
                };

                List<EnumRoles> roleEnumList = appUser.UserRoles.ConvertAll(delegate (string x) { return (EnumRoles)Enum.Parse(typeof(EnumRoles), x); });

                appUser.SelectedRole = roleEnumList.Max().ToString();
                appUser.Login = true;

                session.SetKey<AppUser>(Literals.SessionKey_AppUser, appUser);
            }
            catch (Exception ex)
            {
                session.SetKey<AppUser>(Literals.SessionKey_AppUser, new AppUser());
            }
        }

        public static bool ValidateUserSession(this ISession session)
        {
            AppUser? appUser = session.GetKey<AppUser>(Literals.SessionKey_AppUser);

            if (appUser == null) return false;

            if (appUser.Login == false) return false;
            if (appUser.UserId == null) return false;
            if (appUser.SelectedRole == null) return false;
            if (appUser.UserRoles == null) return false;
            if (appUser.UserRoles.Count == 0) return false;

            return true;
        }

        public static void RemoveUserSession(this ISession session)
        {
            session.Remove(Literals.SessionKey_AppUser);
            session.Clear();
        }
    }
}