using System;

namespace Keebox.SecretsService.Models
{
    [Serializable]
    public record SystemInfo
    {
        public SystemInfo(string version, string storageType, int uptime)
        {
            Version = version;
            StorageType = storageType;
            Uptime = uptime;
        }

        public string Version { init; get; }

        public string StorageType { init; get; }

        public int Uptime { init; get; }
    }
}