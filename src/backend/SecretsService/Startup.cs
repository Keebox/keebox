using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Ralfred.Common.DataAccess.Repositories;
using Ralfred.Common.DataAccess.Repositories.Abstractions;
using Ralfred.Common.DependencyInjection;
using Ralfred.Common.Helpers;
using Ralfred.Common.Helpers.Serialization;
using Ralfred.Common.Managers;
using Ralfred.Common.Types;
using Ralfred.SecretsService.Services;

using Serilog;


namespace Ralfred.SecretsService
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(_configuration, "Serilog")
				.CreateLogger();

			services.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.AddSerilog(Log.Logger);
			});

			services.AddTransient<JsonSerializer>();
			services.AddTransient<XmlSerializer>();
			services.AddTransient<YamlSerializer>();

			services.AddTransient<StorageResolvingExtensions.SerializerResolver>(serviceProvider => key =>
			{
				return key switch
				{
					FormatType.Json => serviceProvider.GetService<JsonSerializer>()!,
					FormatType.Xml  => serviceProvider.GetService<XmlSerializer>()!,
					FormatType.Yaml => serviceProvider.GetService<YamlSerializer>()!,

					_ => null
				};
			});

			services.AddTransient<IPathResolver, PathResolver>();
			services.AddTransient<IFileConverter, FileConverter>();
			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<ISecretsManager, SecretsManager>();
			services.AddTransient<ITokenService, TokenService>();
			services.AddTransient<IAccountManager, AccountManager>();
			services.AddTransient<IFormatterResolver, FormatterResolver>();

			services.AddTransient<IContentManager, ContentManager>();

			services.AddTransient<IConfigurationManager, ConfigurationManager>(serviceProvider =>
				new ConfigurationManager(serviceProvider.GetService<YamlSerializer>()!, serviceProvider.GetService<IContentManager>()!,
					serviceProvider.GetService<ITokenService>()!));

			var configuration = RegisterApplicationConfiguration(services);

			services.ConfigureRepositoryContext(configuration);


			services.AddControllers(options =>
			{
				options.Filters.Add<ExceptionsFilter>();
				options.InputFormatters.Add(new BypassFormDataInputFormatter());
			});
		}

		public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			applicationLifetime.ApplicationStopped.Register(() => { Log.Information("Application stopped."); });

			Log.Information("Application started.");
		}

		private static Configuration RegisterApplicationConfiguration(IServiceCollection services)
		{
			// TODO: add config validation

			var serviceProvider = services.BuildServiceProvider();
			var configurationManager = serviceProvider.GetService<IConfigurationManager>()!;

			var appConfigurationDefaults = configurationManager.Get(_configuration!["DefaultSettingsPath"]);
			var appConfigurationOverrides = configurationManager.Get(_configuration!["OverridesSettingsPath"]);

			if (appConfigurationDefaults is null)
			{
				Log.Information("Application configuration is not initialized");
				appConfigurationDefaults = configurationManager.GetDefaultConfiguration();
				configurationManager.Save(_configuration!["DefaultSettingsPath"], appConfigurationDefaults);
				Log.Information($"Here is your root token: {appConfigurationDefaults.RootToken}");
			}

			var appConfiguration = appConfigurationDefaults;

			if (appConfigurationOverrides is not null)
				appConfiguration = configurationManager.Merge(appConfigurationDefaults, appConfigurationOverrides);

			services.AddSingleton(appConfiguration);

			Log.Information("Loading application configuration.");

			return appConfiguration;
		}

		private static IConfiguration? _configuration;
	}
}