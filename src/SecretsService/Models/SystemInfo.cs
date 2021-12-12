using System;


namespace Keebox.SecretsService.Models
{
	[Serializable]
	public record SystemInfo(string Version, string StorageType, int Uptime);
}