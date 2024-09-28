using System;
using Microsoft.AspNetCore.Mvc;
using src.App_Data.Types;
using src.App_Lib.Filters;

namespace src.App_Lib.Attributes;

/// <summary>
/// Rollerden en az 1 tanesi olması gerekir (OR koşulu uygular)
/// </summary>
public class RoleRequirementAttribute : TypeFilterAttribute
{
    public RoleRequirementAttribute(EnumRoles[] AllowedRoles) : base(typeof(RoleRequirementFilter))
    {
        Arguments = new object[] { AllowedRoles };
    }
}
