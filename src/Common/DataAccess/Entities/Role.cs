namespace Keebox.Common.DataAccess.Entities
{
	public record Role : Entity
	{
		public string Name { get; init; }

		public bool IsSystem { get; set; }
	}
}