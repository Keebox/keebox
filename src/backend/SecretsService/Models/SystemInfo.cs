namespace Keebox.SecretsService.Models
{
    public record SystemInfo
    {
        public string Version { init; get; }

        public string StorageType { init; get; }

        public int Runtime { init; get; }
    }
}