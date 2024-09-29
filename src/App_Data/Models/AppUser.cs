namespace src.App_Data.Models;

public record AppUser
{
	public string? SessionID { get; set; } = null;
	public bool Login { get; set; } = false;
	public string? UserId { get; set; } = null;
	public List<string>? UserRoles { get; set; } = null;
	public string? SelectedRole { get; set; } = null;
}