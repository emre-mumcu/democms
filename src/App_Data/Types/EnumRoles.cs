using src.App_Lib.Attributes;

namespace src.App_Data.Types
{
	public enum EnumRoles
	{
		[RoleDescription(Restricted = true, RoleName = "User", RoleDetail = "User")]
		USER = 10,
		[RoleDescription(Restricted = false, RoleName = "Administrator", RoleDetail = "Administrator")]
		ADMINISTRATOR = 100
	}
}