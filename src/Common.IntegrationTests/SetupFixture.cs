using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;


namespace Keebox.Common.IntegrationTests
{
	[SetUpFixture]
	public class SetupFixture
	{
		[OneTimeSetUp]
		public void GlobalSetup()
		{
			var serviceProvider = Setup.CreateServices();
			var postgresMigrationRunner = serviceProvider.GetRequiredService<IMigrationRunner>();

			postgresMigrationRunner.MigrateUp();
		}
	}
}