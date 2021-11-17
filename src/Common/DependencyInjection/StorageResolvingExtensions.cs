using System;
using System.Reflection;

using FluentMigrator.Runner;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.InMemory;
using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Types;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DependencyInjection
{
	public static class StorageResolvingExtensions
	{
		public delegate ISerializer? SerializerResolver(FormatType? format);

		public static void ConfigureRepositoryContext(this IServiceCollection services, Configuration configuration)
		{
			var storageEngine = configuration.Engine!.Value;

			if (storageEngine != StorageEngineType.InMemory)
				services.AddTransient(_ => new StorageConnection
				{
					ConnectionString = configuration.ConnectionString!
				});

			switch (storageEngine)
			{
				case StorageEngineType.InMemory:
				{
					services.AddSingleton<InMemoryAccountRepository>();
					services.AddSingleton<InMemoryGroupRepository>();
					services.AddSingleton<InMemorySecretRepository>();
					services.AddSingleton<InMemoryRoleRepository>();
					services.AddSingleton<InMemoryPermissionRepository>();

					services.AddSingleton<IRepositoryContext, InMemoryRepositoryContext>();

					break;
				}

				case StorageEngineType.Postgres:
				{
					services.AddTransient<IConnectionFactory, ConnectionFactory>();

					services.AddTransient<PostgresAccountRepository>();
					services.AddTransient<PostgresGroupRepository>();
					services.AddTransient<PostgresSecretRepository>();
					services.AddTransient<PostgresRoleRepository>();
					services.AddTransient<PostgresPermissionRepository>();

					services.AddSingleton<IRepositoryContext, PostgresRepositoryContext>();

					services.AddFluentMigratorCore()
						.ConfigureRunner(rb =>
						{
							var storageConnection = rb.Services.BuildServiceProvider().GetRequiredService<StorageConnection>();

							rb.AddPostgres()
								.WithGlobalConnectionString(storageConnection.ConnectionString)
								.ScanIn(Assembly.GetExecutingAssembly()).For.Migrations();
						});

					using (var scope = services.BuildServiceProvider().CreateScope())
					{
						scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
					}

					break;
				}

				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}