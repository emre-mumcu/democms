using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using src.App_Data.Types;
using src.App_Lib.Abstract;
using src.App_Lib.Configuration;

namespace src.App_Lib.Concrete;

public class TestAuthorize : IAuthorize
{
	private ClaimsPrincipal GetPrincipal(string UserId)
	{
		try
		{
			List<Claim> UserClaims = new List<Claim>() {
					new Claim(ClaimTypes.NameIdentifier, UserId),
					new Claim(ClaimTypes.Name, "John"),
					new Claim(ClaimTypes.Surname, "Doe"),
					new Claim(ClaimTypes.Role, EnumRoles.ADMINISTRATOR.ToString()),
					new Claim(ClaimTypes.Role, EnumRoles.USER.ToString())
			};

			return new ClaimsPrincipal(new ClaimsIdentity(UserClaims, CookieAuthenticationDefaults.AuthenticationScheme));
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	private AuthenticationProperties GetProperties() => new AuthenticationProperties
	{
		AllowRefresh = Literals.Auth_Prop_Allow_Session_Refresh,
		ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Literals.Auth_Prop_Ticket_Expire),
		IsPersistent = Literals.Auth_Prop_Session_Is_Persistent,
		IssuedUtc = DateTimeOffset.UtcNow,
		RedirectUri = Literals.Auth_Prop_RedirectUri
	};

	public AuthenticationTicket GetTicket(string userId) => new AuthenticationTicket(GetPrincipal(userId), GetProperties(), CookieAuthenticationDefaults.AuthenticationScheme);
}