namespace src.Areas.Admin.Models;

public class AppRole
{
	public required string TypeName { get; set; }
	public required string Namespace { get; set; }
	public required string FullName { get; set; }
	public required string Name { get; set; }
	public required string MemberName { get; set; }
	public required string ReturnType { get; set; }
	public required string GuidString { get; set; }
}