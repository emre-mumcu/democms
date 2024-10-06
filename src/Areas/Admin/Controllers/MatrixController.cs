using Microsoft.AspNetCore.Mvc;
using src.App_Data;
using src.App_Data.Types;
using src.App_Lib.Attributes;
using src.Areas.Admin.ViewModels;

namespace src.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class MatrixController : Controller
    {
		[RoleRequirement(AllowedRoles: new EnumRoles[] { EnumRoles.ADMINISTRATOR })]
		public IActionResult Index([FromServices] AppDbContext appDbContext)
		{
			MatrixVM vm = new MatrixVM();
			vm.AppRoles = new UygulamaYetkileri().YetkiListesi();
			vm.DynamicRoles = appDbContext.RoleMatrixes.ToList();

			return View(model: vm);
		}

	}
}
