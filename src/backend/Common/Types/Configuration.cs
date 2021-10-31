using System;


namespace Keebox.Common.Types
{
	[Serializable]
	public record Configuration
	{
		public bool? EnableWebUi { get; set; }

		public FormatType? DefaultFormat { get; set; }

		public StorageEngineType? Engine { get; set; }

		public string? ConnectionString { get; set; }

		public string? RootToken { get; set; }
	}
}