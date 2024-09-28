using src.App_Data.Types;

namespace src.App_Data.Entities
{
	public class BaseEntity : IEntity
	{
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public bool State { get; set; } = true;
		public EnumStatus Status { get; set; } = EnumStatus.Created;
	}
}