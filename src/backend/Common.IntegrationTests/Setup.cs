using System;
using System.Reflection;

using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.IntegrationTests
{
	public static class Setup
	{
		public static IServiceProvider CreateServices()
		{
			var referencedAssembly = Assembly.Load("Keebox.Common");

			return new ServiceCollection()
				.AddFluentMigratorCore()
				.ConfigureRunner(rb =>
				{
					rb.AddPostgres()
						.WithGlobalConnectionString(ConfigurationHelper.PostgresDatabaseConnectionString)
						.ScanIn(referencedAssembly).For.Migrations();
				})
				.BuildServiceProvider(false);
		}
	}
}