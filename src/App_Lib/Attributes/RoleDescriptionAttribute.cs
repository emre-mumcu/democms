namespace src.App_Lib.Attributes;

public class RoleDescriptionAttribute : Attribute
{
	public bool Restricted { get; set; }
	public required string RoleName { get; set; }
	public required string RoleDetail { get; set; }
}