using DataAccess.Entities;
using DataAccess.Extensions;

using FluentMigrator;


namespace DataAccess.Migrations;

[Migration(1)]
public sealed class SecretsMigration : AutoReversingMigration
{
	public override void Up()
	{
		Create
			.Table("group")
			.InSchema("public")
			.WithColumn(nameof(Group.Id).ToSnakeCase()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
			.WithColumn(nameof(Group.Path).ToSnakeCase()).AsString(512).NotNullable()
			.WithColumn(nameof(Group.CreatedAt).ToSnakeCase()).AsDateTime().NotNullable()
			.WithDefaultValue(SystemMethods.CurrentUTCDateTime);

		Create
			.Table("secret")
			.InSchema("public")
			.WithColumn(nameof(Secret.Id).ToSnakeCase()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
			.WithColumn(nameof(Secret.Name).ToSnakeCase()).AsString(256).NotNullable()
			.WithColumn(nameof(Secret.Value).ToSnakeCase()).AsString().Nullable()
			.WithColumn(nameof(Secret.File).ToSnakeCase()).AsBinary().Nullable()
			.WithColumn(nameof(Secret.CreatedAt).ToSnakeCase()).AsDateTime().NotNullable()
			.WithDefaultValue(SystemMethods.CurrentUTCDateTime)
			.WithColumn(nameof(Secret.GroupId).ToSnakeCase()).AsGuid().NotNullable();

		Create.ForeignKey().FromTable("secret").InSchema("public").ForeignColumn(nameof(Secret.GroupId).ToSnakeCase()).ToTable("group")
			.InSchema("public").PrimaryColumn(nameof(Group.Id).ToSnakeCase());
	}
}