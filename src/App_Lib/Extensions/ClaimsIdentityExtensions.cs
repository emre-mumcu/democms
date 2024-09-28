using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace src.App_Lib.Extensions
{


    public static class ClaimsIdentityExtensions
    {
        public static bool HasClaim(this ClaimsIdentity ci, Claim claim)
        {
            return ci.GetRoles().Any(c => c == claim);
        }

        public static bool HasRole(this ClaimsIdentity ci, string roleName)
        {
            return ci.GetRoles().Any(c => c.Value == roleName);
        }

        public static string GetNameIdentifier(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
        }

        public static string GetDepartment(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == "KullaniciPersonelUniteKod").First().Value;
        }

        public static string GetSid(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == ClaimTypes.Sid).First().Value;
        }

        public static string GetName(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == ClaimTypes.Name).First().Value;
        }

        public static string GetSurname(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == ClaimTypes.Surname).First().Value;
        }

        public static string GetEmail(this ClaimsIdentity ci)
        {
            return ci.Claims.Where(c => c.Type == ClaimTypes.Email).First().Value;
        }

        public static string GetValue(this ClaimsIdentity ci, string claimType)
        {
            return ci.Claims.Where(c => c.Type == claimType).First().Value;
        }

        public static List<Claim> GetRoles(this ClaimsIdentity ci)
        {
            if (ci is null) return new List<Claim>();
            return ci.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        }
    }

}