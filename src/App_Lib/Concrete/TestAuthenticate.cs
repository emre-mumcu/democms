using src.App_Lib.Abstract;

namespace src.App_Lib.Concrete;

public class TestAuthenticate : IAuthenticate
{
	public bool AuthenticateUser(string User, string Password)
	{
		return true;
	}
}