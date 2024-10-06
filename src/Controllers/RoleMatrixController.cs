using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;
using src.App_Data.Types;
using src.App_Lib.Attributes;
using src.App_Lib.Cache;
using src.App_Lib.Extensions;
using src.App_Lib.Tools;
using src.Areas.Admin.Models;
using src.Areas.Admin.ViewModels;

namespace src.Controllers;

public class RoleMatrixController : Controller
{
    [RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
    public IActionResult YetkiMatrisi([FromServices] AppDbContext appDbContext)
    {
		MatrixVM vm = new MatrixVM();
        vm.AppRoles = new UygulamaYetkileri().YetkiListesi();
        vm.DynamicRoles = appDbContext.RoleMatrixes.ToList();

        return View(model: vm);
    }

    [RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
    public async void YetkiIslem([FromServices] AppDbContext appDbContext, string Yetki, string Role, int Durum)
    {
        string roleName = Security.Decrypt(Role, HttpContext.Session.Id);
        bool roleDurum = Durum == 1;

        // check if yetli && role defined earlier:
        DynamicRole? mevcutYetki = appDbContext.RoleMatrixes.Where(y => y.FullName == Yetki && y.RoleCode == roleName).FirstOrDefault();

        if (mevcutYetki is null)
        {
            DynamicRole yetki = new DynamicRole()
            {
                RoleCode = roleName,
                FullName = Yetki,
                Allow = roleDurum,
                DepartmentId = (User.Identity as ClaimsIdentity).GetDepartment(),
                UserName = (User.Identity as ClaimsIdentity).GetNameIdentifier()
            };

            appDbContext.RoleMatrixes.Add(yetki);
            appDbContext.SaveChanges();
        }
        else
        {
            mevcutYetki.Allow = roleDurum;
            mevcutYetki.DepartmentId = (User.Identity as ClaimsIdentity).GetDepartment();
            mevcutYetki.UserName = (User.Identity as ClaimsIdentity).GetNameIdentifier();

            appDbContext.RoleMatrixes.Update(mevcutYetki);
            appDbContext.SaveChanges();
        }

        // Update Cache
		var memCache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();		
        await new _CacheRoleMatrix(memCache).GetCache(isDirty: true);
    }

}

public class UygulamaYetkileri
{
    public List<AppRole> YetkiListesi()
    {
        Assembly? asm = Assembly.GetAssembly(typeof(Program));

        var controllers = asm?.GetTypes()
            .Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))
            .Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
            .Select(i => new AppRole
			{
                TypeName = i?.BaseType.FullName,
                Namespace = i?.Namespace,
                FullName = i?.FullName,
                Name = i?.Name,
                MemberName = "*",
                ReturnType = "NOT-APPLICABLE",
                GuidString = i?.GUID.ToString()
            })
            .ToList();

        var actions = asm?.GetTypes()
            .Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            .Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
            .Select(i => new AppRole
			{
                TypeName = i?.MemberType.ToString(),
                Namespace = i?.DeclaringType?.Namespace,
                FullName = i?.DeclaringType?.FullName,
                Name = i?.DeclaringType?.Name,
                MemberName = i?.Name,
                ReturnType = i?.ReturnType.Name,
                GuidString = i?.ReflectedType.GUID.ToString()
            })
            .ToList();

        var pages = asm?.GetTypes()
            .Where(type => typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageModel).IsAssignableFrom(type))
            .Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
            .Select(i => new AppRole
			{
                TypeName = i?.MemberType.ToString(),
                Namespace = i?.Namespace,
                FullName = i?.FullName,
                Name = i?.Name,
                MemberName = "*",
                ReturnType = "NOT-APPLICABLE",
                GuidString = i?.GUID.ToString()
            })
            .ToList();

        List<AppRole> allItems = controllers.Concat(actions).Concat(pages).ToList();

        return allItems;
    }
}

