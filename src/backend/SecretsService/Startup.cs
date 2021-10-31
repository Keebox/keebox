using Keebox.Common.DependencyInjection;
using Keebox.Common.Helpers;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Managers;
using Keebox.Common.Types;
using Keebox.SecretsService.RequestFiltering;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;


namespace Keebox.SecretsService
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
					FormatType.Json => serviceProvider.GetRequiredService<JsonSerializer>(),
					FormatType.Xml  => serviceProvider.GetRequiredService<XmlSerializer>(),
					FormatType.Yaml => serviceProvider.GetRequiredService<YamlSerializer>(),

					_ => null
				};
			});

			services.AddTransient<IPathResolver, PathResolver>();
			services.AddTransient<IFileConverter, FileConverter>();

			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<ISecretsManager, SecretsManager>();
			services.AddTransient<ITokenService, TokenService>();

			services.AddTransient<ISecretsManager, SecretsManager>();
			services.AddTransient<IAccountManager, AccountManager>();

			services.AddTransient<IFormatterResolver, FormatterResolver>();

			services.AddTransient<IContentManager, ContentManager>();

			services.AddTransient<IConfigurationManager, ConfigurationManager>(serviceProvider =>
				new ConfigurationManager(serviceProvider.GetRequiredService<YamlSerializer>(),
					serviceProvider.GetRequiredService<IContentManager>(),
					serviceProvider.GetRequiredService<ITokenService>()));

			var configuration = RegisterApplicationConfiguration(services);

			services.ConfigureRepositoryContext(configuration);

			services.AddTransient<ITokenValidator, TokenValidator>();

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
			var configurationManager = serviceProvider.GetRequiredService<IConfigurationManager>();

			var appConfigurationDefaults = configurationManager.Get(_configuration!["DefaultSettingsPath"]);
			var appConfigurationOverrides = configurationManager.Get(_configuration["OverridesSettingsPath"]);

			if (appConfigurationDefaults is null)
			{
				Log.Information("Application configuration is not initialized");

				appConfigurationDefaults = configurationManager.GetDefaultConfiguration();
				configurationManager.Save(_configuration["DefaultSettingsPath"], appConfigurationDefaults);

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