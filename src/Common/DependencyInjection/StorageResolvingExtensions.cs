using System;
using System.Reflection;

using FluentMigrator.Runner;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.InMemory;
using Keebox.Common.DataAccess.Repositories.InMemory.Migrations.Runner;
using Keebox.Common.DataAccess.Repositories.Postgres;
using Keebox.Common.Helpers.Serialization;
using Keebox.Common.Types;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DependencyInjection
{
	public static class StorageResolvingExtensions
	{
		public delegate ISerializer? SerializerResolver(FormatType? format);

		public static IServiceProvider ConfigureRepositoryContext(this IServiceCollection services, Configuration configuration)
		{
			var storageEngine = configuration.Engine!.Value;

			if (storageEngine != StorageEngineType.InMemory)
				services.AddTransient(_ => new StorageConnection
				{
					ConnectionString = configuration.ConnectionString!
				});

			IServiceProvider serviceProvider;

			switch (storageEngine)
			{
				case StorageEngineType.InMemory:
				{
					services.AddSingleton<InMemoryAccountRepository>();
					services.AddSingleton<InMemoryGroupRepository>();
					services.AddSingleton<InMemorySecretRepository>();
					services.AddSingleton<InMemoryRoleRepository>();
					services.AddSingleton<InMemoryPermissionRepository>();
					services.AddSingleton<InMemoryAssignmentRepository>();

					services.AddSingleton<IRepositoryContext, InMemoryRepositoryContext>();

					services.AddInMemoryMigrations();

					serviceProvider = services.BuildServiceProvider();

					serviceProvider.GetRequiredService<InMemoryMigrationRunner>().MigrateUp(serviceProvider);

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
					services.AddSingleton<PostgresAssignmentRepository>();

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

					serviceProvider = services.BuildServiceProvider();

					break;
				}

				default: throw new ArgumentOutOfRangeException();
			}

			return serviceProvider;
		}
	}
}