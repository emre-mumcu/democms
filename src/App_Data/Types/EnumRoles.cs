using src.App_Lib.Attributes;

namespace src.App_Data.Types
{
    public enum EnumRoles
    {
        [RoleDescription(Restricted = true, RoleName = "User", RoleDetail = "Standart User")]
        USER = 10,
        [RoleDescription(Restricted = false, RoleName = "Administrator", RoleDetail = "System Administrator")]
        ADMINISTRATOR = 100
    }
}