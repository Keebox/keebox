using Microsoft.Extensions.Configuration;


namespace Keebox.Common.Helpers
{
	public class KeyProvider : IKeyProvider
	{
		public KeyProvider(IConfiguration configuration, IContentManager contentManager)
		{
			_configuration = configuration;
			_contentManager = contentManager;
		}

		public byte[] GetTokenSigningKey()
		{
			return _contentManager.GetRaw(_configuration["SigningKeyPath"])!;
		}

		private readonly IContentManager _contentManager;
		private readonly IConfiguration _configuration;
	}
}