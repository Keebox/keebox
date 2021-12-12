namespace Keebox.Common.DataAccess.Entities
{
	public sealed record Group : Entity
	{
		public string Name { get; init; } = default!;

		public string Path { get; init; } = default!;
	}
}