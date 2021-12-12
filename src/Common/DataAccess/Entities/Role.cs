namespace Keebox.Common.DataAccess.Entities
{
	public sealed record Role : Entity
	{
		public string Name { get; init; } = default!;

		public bool IsSystem { get; init; }
	}
}