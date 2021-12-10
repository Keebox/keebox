using System;

using Keebox.Common.Helpers;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Types;


namespace Keebox.Common.Managers
{
	public class ConfigurationManager : IConfigurationManager
	{
		private readonly IContentManager _contentManager;

		private readonly ISerializer _serializer;
		private readonly ITokenService _tokenService;

		public ConfigurationManager(ISerializer serializer, IContentManager contentManager, ITokenService tokenService)
		{
			_serializer = serializer;
			_contentManager = contentManager;
			_tokenService = tokenService;
		}

		public Configuration? Get(string path)
		{
			var content = _contentManager.Get(path);

			if (content is null)
				return default;

			return _serializer.Deserialize<Configuration>(content);
		}

		public Configuration Merge(Configuration old, Configuration @new)
		{
			old.Engine = @new.Engine ?? old.Engine;
			old.EnableWebUi = @new.EnableWebUi ?? old.EnableWebUi;
			old.DefaultFormat = @new.DefaultFormat ?? old.DefaultFormat;
			old.ConnectionString = @new.ConnectionString ?? old.ConnectionString;

			return old;
		}

		public Configuration GetDefaultConfiguration()
		{
			return new Configuration
			{
				Engine = StorageEngineType.InMemory,
				DefaultFormat = FormatType.Json,
				RootToken = _tokenService.GenerateToken(),
				EnableWebUi = true
			};
		}

		public void Save(string path, Configuration configuration)
		{
			var content = _serializer.Serialize(configuration);
			_contentManager.Save(path, content!);
		}
	}
}