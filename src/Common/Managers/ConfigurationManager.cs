using Keebox.Common.Helpers;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Types;


namespace Keebox.Common.Managers
{
	public class ConfigurationManager : IConfigurationManager
	{
		public ConfigurationManager(ISerializer serializer, IContentManager contentManager)
		{
			_serializer = serializer;
			_contentManager = contentManager;
		}

		public Configuration? Get(string path)
		{
			var content = _contentManager.Get(path);

			if (content is null) return default;

			return _serializer.Deserialize<Configuration>(content);
		}

		public Configuration Merge(Configuration old, Configuration @new)
		{
			old.Engine = @new.Engine ?? old.Engine;
			old.RootToken = @new.RootToken ?? old.RootToken;
			old.EnableWebUi = @new.EnableWebUi ?? old.EnableWebUi;
			old.DefaultFormat = @new.DefaultFormat ?? old.DefaultFormat;
			old.ConnectionString = @new.ConnectionString ?? old.ConnectionString;

			return old;
		}

		public Configuration GetDefaultConfiguration()
		{
			return new Configuration
			{
				Status = Status.NotInitialized,
				Engine = StorageEngineType.InMemory,
				DefaultFormat = FormatType.Json,
				EnableWebUi = true
			};
		}

		public void Save(string path, Configuration configuration)
		{
			var content = _serializer.Serialize(configuration);
			_contentManager.Save(path, content!);
		}

		private readonly IContentManager _contentManager;
		private readonly ISerializer _serializer;
	}
}