using System;
using System.Linq;
using System.Reflection;

using Serilog;


namespace Keebox.Common.DataAccess.Repositories.InMemory.Migrations.Runner;

public class InMemoryMigrationRunner
{
	public void MigrateUp(IServiceProvider serviceProvider)
	{
		var assemblyTypes = Assembly.GetAssembly(typeof(InMemoryMigration))?.GetTypes();

		var migrationTypes = assemblyTypes?
			.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InMemoryMigration)))
			.ToArray();

		var migrations = migrationTypes?.Select(type => (InMemoryMigration)Activator.CreateInstance(type)!).ToArray();

		if (migrations == null) return;

		foreach (var migration in migrations)
		{
			migration.ServiceProvider = serviceProvider;

			var migrationType = migration.GetType();

			var migrateUpMethod = migrationType.GetMethod(
				"Up", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder,
				Array.Empty<Type>(), null);

			Log.Information($"Applying migration {migration.GetType().Name}");

			migrateUpMethod?.Invoke(migration, Array.Empty<object?>());
		}
	}
}