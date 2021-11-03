using Keebox.Common.Types;

namespace Keebox.SecretsService.Models
{
    public record Config
    {
        public bool EnableWebUi { get; set; }

        public string DefaultFormat { get; set; }

        public string Engine { get; set; }
    }
}