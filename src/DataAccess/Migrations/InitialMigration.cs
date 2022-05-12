using FluentMigrator;


namespace DataAccess.Migrations;

[Migration(0)]
public sealed class UuidMigration : ForwardOnlyMigration
{
	public override void Up()
	{
		Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"");
	}
}