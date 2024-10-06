using Microsoft.AspNetCore.Mvc;
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
		[RoleRequirement(AllowedRoles: [EnumRoles.ADMINISTRATOR])]
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
		[RoleRequirement(AllowedRoles: [EnumRoles.ADMINISTRATOR])]
		public async void Index([FromServices] AppDbContext appDbContext, string Yetki, string Role, int Durum)
		{
			string roleName = Security.Decrypt(Role, HttpContext.Session.Id);
			
			DynamicRole? role = appDbContext.DynamicRoles
				.Where(y => y.FullName == Yetki && y.RoleCode == roleName)
				.FirstOrDefault()
			;

			if (role == null)
			{
				DynamicRole yetki = new DynamicRole()
				{
					RoleCode = roleName,
					FullName = Yetki,
					Allow = Durum == 1,
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
				};

				appDbContext.DynamicRoles.Add(yetki);				
			}
			else
			{
				role.Allow = Durum == 1;
				role.Updated = DateTime.UtcNow;

				appDbContext.DynamicRoles.Update(role);
			}
			
			appDbContext.SaveChanges();
			
			// Update Cache			
			await new _CacheRoleMatrix(HttpContext.RequestServices.GetRequiredService<IMemoryCache>()).GetCache(isDirty: true);
		}
	}
}