using System;

namespace src.App_Data.LookUps;

public class State 
{
    public int Id { get; set; }
    public string StateName { get; set; } = null!;
    public ICollection<City>? Cities { get; set; }
}
