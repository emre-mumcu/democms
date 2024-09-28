namespace src.App_Data.LookUps;

public class City
{
	public int Id { get; set; }
	public int StateId { get; set; }
	public required string CityName { get; set; }
	public decimal Population { get; set; }
	public decimal Area { get; set; }
}