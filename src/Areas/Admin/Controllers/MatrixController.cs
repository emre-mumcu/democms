using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;
using src.App_Data.Types;
using src.App_Lib.Attributes;
using src.App_Lib.Cache;
using src.App_Lib.Concrete;
using src.App_Lib.Extensions;
using src.App_Lib.Tools;
using src.Areas.Admin.ViewModels;

namespace src.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class MatrixController : Controller
    {
		[HttpGet]
		[RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
		public IActionResult Index([FromServices] AppDbContext appDbContext)
		{
			var vals = EnumExtensions.GetAllValues<EnumRoles>();

			MatrixVM vm = new MatrixVM() 
			{
				AppRoles = new AppRoles().GetAll(),
				DynamicRoles = appDbContext.DynamicRoles.ToList(),
				RoleDescriptionAttributes = EnumExtensions.GetAllAttributes<EnumRoles, RoleDescriptionAttribute>()
			};

			return View(model: vm);
		}

		[HttpPost]
		[RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
		public async void Index([FromServices] AppDbContext appDbContext, string Yetki, string Role, int Durum)
		{
			string roleName = Security.Decrypt(Role, HttpContext.Session.Id);
			bool roleDurum = Durum == 1;

			// check if yetli && role defined earlier:
			DynamicRole? mevcutYetki = appDbContext.DynamicRoles.Where(y => y.FullName == Yetki && y.RoleCode == roleName).FirstOrDefault();

			if (mevcutYetki is null)
			{
				DynamicRole yetki = new DynamicRole()
				{
					RoleCode = roleName,
					FullName = Yetki,
					Allow = roleDurum,
				};

				appDbContext.DynamicRoles.Add(yetki);
				appDbContext.SaveChanges();
			}
			else
			{
				mevcutYetki.Allow = roleDurum;

				appDbContext.DynamicRoles.Update(mevcutYetki);
				appDbContext.SaveChanges();
			}

			// Update Cache
			var memCache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
			await new _CacheRoleMatrix(memCache).GetCache(isDirty: true);
		}

	}
}