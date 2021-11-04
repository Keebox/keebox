namespace Keebox.Common.Types
{
	public record StorageConnection
	{
		public string ConnectionString { get; init; } = default!;
	}
}