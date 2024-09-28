using src.App_Data.Types;

namespace src.App_Data.Entities;

public interface IEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public bool State { get; set; }
    public EnumStatus Status { get; set; }
}