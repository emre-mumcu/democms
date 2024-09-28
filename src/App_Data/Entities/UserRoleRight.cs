namespace src.App_Data.Entities;

public class RoleMatrix : BaseEntity
{
	public required string RoleCode { get; set; }
	public required string FullName { get; set; }
	public required string DepartmentId { get; set; }
	public required string UserName { get; set; }
	public bool Allow { get; set; } = true;
}