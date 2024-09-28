using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace src.App_Data.Types;

public enum EnumCacheNames
{
	[Display(Name = "City Cache")]
	[Description("City Cache")]
	City = 1,
	[Display(Name = "State Cache")]
	[Description("State Cache")]
	State = 2,
	[Display(Name = "Gender Cache")]
	[Description("Gender Cache")]
	Gender = 3
}