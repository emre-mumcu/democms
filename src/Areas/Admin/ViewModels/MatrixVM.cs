using src.App_Data.Entities;
using src.Areas.Admin.Models;

namespace src.Areas.Admin.ViewModels;

public class MatrixVM
{
	public List<AppRole>? AppRoles { get; set; }
	public List<DynamicRole>? DynamicRoles { get; set; }
}