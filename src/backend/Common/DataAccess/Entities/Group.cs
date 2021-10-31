namespace Keebox.Common.DataAccess.Entities
{
	public record Group : Entity
	{
		public string Name { get; init; }

		public string Path { get; init; }

		// TODO: add secret ids
	}
}