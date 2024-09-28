using System;

namespace src.App_Data.LookUps;

public class City
{
    public int Id { get; set; }
    public int StateId { get; set; }
    public string CityName { get; set; } = null!;
    public decimal Population { get; set; }
    public decimal Area { get; set; }
}
