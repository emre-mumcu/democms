namespace src.App_Data.LookUps;

public class State
{
	public int Id { get; set; }
	public required string StateName { get; set; }
	public ICollection<City>? Cities { get; set; }
}