namespace src.App_Data.Entities;

public class DynamicRole : BaseEntity
{
	public required string RoleCode { get; set; }
	public required string FullName { get; set; }
	public bool Allow { get; set; } = true;
}