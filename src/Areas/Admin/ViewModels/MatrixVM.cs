using src.App_Data.Entities;
using src.App_Lib.Attributes;
using src.Areas.Admin.Models;

namespace src.Areas.Admin.ViewModels;

public class MatrixVM
{
	public IEnumerable<AppRole>? AppRoles { get; set; }
	public IEnumerable<DynamicRole>? DynamicRoles { get; set; }
	public IEnumerable<RoleDescriptionAttribute>? RoleDescriptionAttributes { get; set; }
}