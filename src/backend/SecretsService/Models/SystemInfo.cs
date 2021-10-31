namespace Keebox.SecretsService.Models
{
	public record SystemInfo
	{
		public string Version { get; set; }

		public string StorageType { get; set; }
	}
}