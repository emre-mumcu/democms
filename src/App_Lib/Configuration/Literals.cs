namespace src.App_Lib.Configuration;

public static class Literals
{
	public static string TraceId = nameof(TraceId);

	// SESSION        
	public static string Session_Cookie_Name = "app.session.cookie";
	public static TimeSpan Session_IdleTimeout = TimeSpan.FromMinutes(20);
	public const string SessionKey_AppUser = "AppUser";


	// CLAIMS
	public static string Claims_Role_Seperator = ";";
	public static string Claims_Departmant_Name = "Department";

	// AUTHENTICATION
	public static string Auth_Cookie_Name = "app.authentication.cookie";
	public static string Auth_Cookie_LoginPath = "/Login";
	public static string Auth_Cookie_LogoutPath = "/Logout";
	public static string Auth_Cookie_AccessDeniedPath = "/AccessDenied";
	public static string Auth_Cookie_ClaimsIssuer = "app.issuer";
	public static string Auth_Cookie_ReturnUrlParameter = "ReturnUrl";
	public static TimeSpan Auth_Cookie_ExpireTimeSpan = TimeSpan.FromMinutes(20);

	// AuthenticationProperties
	public static double Auth_Prop_Ticket_Expire = 20;
	public static bool Auth_Prop_Session_Is_Persistent = true;
	public static bool Auth_Prop_Allow_Session_Refresh = true;
	public static string Auth_Prop_RedirectUri = "RedirectUri";

	// SESSION KEYS
	public static string SessionKey_Login = "LOGIN";
	public static string SessionKey_Captcha = "CAPTCHA";
	public static string SessionKey_LoginUser = "LOGINUSER";
	public static string SessionKey_SelectedRole = "SELECTED_ROLE";
	public static string SessionKey_System_Message = "SYSTEM_MESSAGE";
}