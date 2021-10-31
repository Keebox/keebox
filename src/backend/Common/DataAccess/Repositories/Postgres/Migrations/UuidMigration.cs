using FluentMigrator;


namespace Keebox.Common.DataAccess.Repositories.Postgres.Migrations
{
	[Migration(0)]
	public class UuidMigration : ForwardOnlyMigration
	{
		public override void Up()
		{
			Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"");
		}
	}
}