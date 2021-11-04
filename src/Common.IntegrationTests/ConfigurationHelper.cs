using Microsoft.Extensions.Configuration;


namespace Keebox.Common.IntegrationTests
{
	public static class ConfigurationHelper
	{
		static ConfigurationHelper() =>
			_configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", false, true)
				.Build();

		public static string PostgresDatabaseConnectionString => _configuration.GetConnectionString("PostgresDatabase");

		private static readonly IConfiguration _configuration;
	}
}