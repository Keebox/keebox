using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Ralfred.Common.DataAccess.Repositories;
using Ralfred.Common.DependencyInjection;
using Ralfred.Common.Helpers;
using Ralfred.Common.Managers;
using Ralfred.Common.Types;
using Ralfred.SecretsProvider.Services;


namespace Ralfred.SecretsProvider
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			/* if json serializer will appear, need to convert to named registration */
			services.AddTransient<ISerializer, YamlSerializer>();
			services.AddTransient<IContentProvider, ContentProvider>();

			services.AddTransient<IConfigurationManager, ConfigurationManager>();

			var configuration = RegisterApplicationConfiguration(services);

			services.ConfigureStorageContext(configuration.Engine!.Value);

			services.AddTransient<ISecretsRepository, SecretsRepository>();
			services.AddTransient<IGroupRepository, GroupRepository>();

			services.AddTransient<IPathResolver, PathResolver>();
			services.AddTransient<IFileConverter, FileConverter>();
			services.AddTransient<ISecretsManager, SecretsManager>();

			services.AddControllers(options => { options.InputFormatters.Add(new BypassFormDataInputFormatter()); });
		}

		public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}

		private static Configuration RegisterApplicationConfiguration(IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider();
			var configurationManager = serviceProvider.GetService<IConfigurationManager>()!;

			var appConfigurationDefaults = configurationManager.Get(_configuration!["DefaultSettingsPath"]);
			var appConfigurationOverrides = configurationManager.Get(_configuration!["OverridesSettingsPath"]);

			if (appConfigurationDefaults is null)
				throw new Exception("Cannot read configuration file. Application stopped.");

			var appConfiguration = appConfigurationDefaults;

			if (appConfigurationOverrides is not null)
				appConfiguration = configurationManager.Merge(appConfigurationDefaults, appConfigurationOverrides);

			services.AddSingleton(appConfiguration);

			return appConfiguration;
		}

		private static IConfiguration? _configuration;
	}
}