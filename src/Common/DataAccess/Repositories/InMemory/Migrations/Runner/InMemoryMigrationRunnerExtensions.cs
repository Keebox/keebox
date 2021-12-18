using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DataAccess.Repositories.InMemory.Migrations.Runner;

public static class InMemoryMigrationRunnerExtensions
{
	public static void AddInMemoryMigrations(this IServiceCollection services)
	{
		services.AddSingleton<InMemoryMigrationRunner>();
	}
}