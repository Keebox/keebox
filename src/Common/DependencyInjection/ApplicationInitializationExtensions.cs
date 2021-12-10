using System;
using System.IO;
using System.Linq;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.Helpers;
using Keebox.Common.Managers;
using Keebox.Common.Types;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;


namespace Keebox.Common.DependencyInjection
{
	public static class ApplicationInitializationExtensions
	{
		public static Configuration RegisterApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			// TODO: add config validation

			var serviceProvider = services.BuildServiceProvider();
			var configurationManager = serviceProvider.GetRequiredService<IConfigurationManager>();

			var appConfigurationDefaults = configurationManager.Get(configuration["DefaultSettingsPath"]);
			var appConfigurationOverrides = configurationManager.Get(configuration["OverridesSettingsPath"]);

			if (appConfigurationDefaults is null)
			{
				Log.Information("Application configuration is not initialized");

				appConfigurationDefaults = configurationManager.GetDefaultConfiguration();
				configurationManager.Save(configuration["DefaultSettingsPath"], appConfigurationDefaults);
			}

			var appConfiguration = appConfigurationDefaults;

			if (appConfigurationOverrides is not null)
				appConfiguration = configurationManager.Merge(appConfigurationDefaults, appConfigurationOverrides);

			Log.Information("Loading application configuration.");

			return appConfiguration;
		}

		public static void RegisterTokenSigningKeys(this IServiceCollection services, IConfiguration configuration)
		{
			var signingKeyPath = configuration["SigningKeyPath"];
			var signingKeyLength = int.Parse(configuration["SigningKeyLength"]);

			if (File.Exists(signingKeyPath)) return;

			var serviceProvider = services.BuildServiceProvider();
			var contentManager = serviceProvider.GetRequiredService<IContentManager>();

			Log.Information("Generating signing keys.");

			var secretKeyRawBuffer = new byte[signingKeyLength];
			new Random().NextBytes(secretKeyRawBuffer);

			contentManager.Save(signingKeyPath, secretKeyRawBuffer);
		}

		public static Configuration SetAdminAccessTokenIfNotPersist(
			this IServiceCollection services, Configuration appConfiguration, IConfiguration serviceProviderConfiguration)
		{
			if (appConfiguration.RootToken is not null) return appConfiguration;

			var serviceProvider = services.BuildServiceProvider();

			var tokenService = serviceProvider.GetRequiredService<ITokenService>();
			var configurationManager = serviceProvider.GetRequiredService<IConfigurationManager>();
			var roleRepository = serviceProvider.GetRequiredService<IRepositoryContext>().GetRoleRepository();

			var adminRoleName = Enum.GetName(typeof(SystemRole), SystemRole.Admin)!.ToLower();

			var adminRole = roleRepository.List()
				.Single(r => r.IsSystem && r.Name.Equals(adminRoleName, StringComparison.OrdinalIgnoreCase));

			var adminToken = tokenService.GenerateJwtToken(Guid.Empty, new[] { adminRole });
			var newAppConfiguration = configurationManager.Merge(appConfiguration, new Configuration { RootToken = adminToken });

			configurationManager.Save(serviceProviderConfiguration["DefaultSettingsPath"], newAppConfiguration);

			return newAppConfiguration;
		}
	}
}