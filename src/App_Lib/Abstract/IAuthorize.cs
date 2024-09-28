using Microsoft.AspNetCore.Authentication;

namespace src.App_Lib.Abstract;

public interface IAuthorize
{
	public AuthenticationTicket GetTicket(string UserId);
}