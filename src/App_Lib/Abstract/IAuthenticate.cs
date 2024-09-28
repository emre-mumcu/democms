namespace src.App_Lib.Abstract;

public interface IAuthenticate
{
	public bool AuthenticateUser(string User, string Password);
}