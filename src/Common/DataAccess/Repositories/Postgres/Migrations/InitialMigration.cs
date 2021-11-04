using FluentMigrator;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Postgres.Migrations
{
	[Migration(1)]
	public class InitialMigration : AutoReversingMigration
	{
		public override void Up()
		{
			Create.Table("account")
				.InSchema("public")
				.WithColumn(nameof(Account.Id).ToLower()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
				.WithColumn(nameof(Account.Name).ToLower()).AsString(256).Nullable()
				.WithColumn(nameof(Account.TokenHash).ToLower()).AsFixedLengthString(128).Nullable()
				.WithColumn(nameof(Account.CertificateThumbprint).ToLower()).AsFixedLengthString(40).Nullable();

			Create
				.Table("group")
				.InSchema("public")
				.WithColumn(nameof(Group.Id).ToLower()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
				.WithColumn(nameof(Group.Name).ToLower()).AsString(256).NotNullable()
				.WithColumn(nameof(Group.Path).ToLower()).AsString(512).NotNullable();

			Create
				.Table("role")
				.InSchema("public")
				.WithColumn(nameof(Role.Id).ToLower()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid);

			Create
				.Table("secret")
				.InSchema("public")
				.WithColumn(nameof(Secret.Id).ToLower()).AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
				.WithColumn(nameof(Secret.Name).ToLower()).AsString(256).NotNullable()
				.WithColumn(nameof(Secret.Value).ToLower()).AsString().NotNullable()
				.WithColumn(nameof(Secret.GroupId).ToLower()).AsGuid().NotNullable()
				.WithColumn(nameof(Secret.IsFile).ToLower()).AsBoolean().NotNullable().WithDefaultValue(false);
		}
	}
}