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

namespace src.Controllers;

public class RoleMatrixController : Controller
{
    // GET: RoleMatrixController
    public ActionResult Index()
    {
        return View();
    }

    [RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
    public IActionResult YetkiMatrisi([FromServices] AppDbContext appDbContext)
    {
        YetkiMatrisiViewModel vm = new YetkiMatrisiViewModel();
        vm.UygulamaYetkiListesi = new UygulamaYetkileri().YetkiListesi();
        vm.VeritabaniYetkiListesi = appDbContext.RoleMatrixes.ToList();

        return View(model: vm);
    }

    [RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
    public async void YetkiIslem([FromServices] AppDbContext appDbContext, string Yetki, string Role, int Durum)
    {
        string roleName = Security.Decrypt(Role, HttpContext.Session.Id);
        bool roleDurum = Durum == 1;

        // check if yetli && role defined earlier:
        RoleMatrix? mevcutYetki = appDbContext.RoleMatrixes.Where(y => y.FullName == Yetki && y.RoleCode == roleName).FirstOrDefault();

        if (mevcutYetki is null)
        {
            RoleMatrix yetki = new RoleMatrix()
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
        await new _CacheRoleMatrix(memCache).GetData(isDirty: true);
    }

}

public class YetkiMatrisiViewModel
{
    public List<RoleItem> UygulamaYetkiListesi { get; set; }
    public List<RoleMatrix> VeritabaniYetkiListesi { get; set; }
}



public class RoleItem
{
    public string TypeName { get; set; }
    public string Namespace { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public string MemberName { get; set; }
    public string ReturnType { get; set; }
    public string GuidString { get; set; }
}

public class UygulamaYetkileri
{
    public List<RoleItem> YetkiListesi()
    {
        Assembly? asm = Assembly.GetAssembly(typeof(Program));

        var controllers = asm?.GetTypes()
            .Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))
            .Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
            .Select(i => new RoleItem
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
            .Select(i => new RoleItem
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
            .Select(i => new RoleItem
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

        List<RoleItem> allItems = controllers.Concat(actions).Concat(pages).ToList();

        return allItems;
    }
}

