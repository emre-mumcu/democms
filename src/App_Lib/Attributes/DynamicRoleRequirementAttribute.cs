using System;
using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Filters;

namespace src.App_Lib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DynamicRoleRequirementAttribute : TypeFilterAttribute
{
    public DynamicRoleRequirementAttribute(Type type, string? name = null) : base(typeof(DynamicRoleRequirementFilter))
    {
        if (name == null) name = "*"; // Alt üye belirtilmez ise, yetkinin tüm alt üyelere uygulanması için * kullanılıyor.

        Arguments = new object[] { type, name };
    }
}
