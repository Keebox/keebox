using System;
using System.IO;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DependencyInjection;
using Keebox.Common.Helpers;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Managers;
using Keebox.Common.Security;
using Keebox.Common.Types;
using Keebox.SecretsService.Middlewares;
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

			services.AddTransient<IDateTimeProvider, DateTimeProvider>();
			services.AddTransient<IKeyProvider, KeyProvider>();
			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<ISecretManager, SecretManager>();

			services.AddTransient<ITokenService, TokenService>();

			services.AddTransient<ISecretManager, SecretManager>();
			services.AddTransient<IAccountManager, AccountManager>();
			services.AddTransient<IRoleManager, RoleManager>();
			services.AddTransient<IPermissionManager, PermissionManager>();

			services.AddTransient<IFormatterResolver, FormatterResolver>();
			services.AddTransient<IContentManager, ContentManager>();

			services.AddTransient<IConfigurationManager, ConfigurationManager>(s =>
				new ConfigurationManager(s.GetRequiredService<YamlSerializer>(), s.GetRequiredService<IContentManager>()));

			services.RegisterTokenSigningKeys(_configuration!);

			var appConfiguration = services.RegisterApplicationConfiguration(_configuration!);
			services.ConfigureRepositoryContext(appConfiguration);

			appConfiguration = services.SetAdminAccessTokenIfNotPersist(appConfiguration, _configuration!);

			services.AddSingleton(appConfiguration);
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

		private static IConfiguration? _configuration;
	}
}