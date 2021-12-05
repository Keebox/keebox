using System;
using System.Collections.Generic;

using FluentMigrator;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Types;


namespace Keebox.Common.DataAccess.Repositories.Postgres.Migrations
{
	[Migration(2)]
	public sealed class SystemRolesMigration : AutoReversingMigration
	{
		public override void Up()
		{
			Insert
				.IntoTable("role")
				.InSchema("public")
				.Row(new Dictionary<string, object>
				{
					{ nameof(Role.Id).ToLower(), Guid.NewGuid().ToString() },
					{ nameof(Role.Name).ToLower(), Enum.GetName(typeof(SystemRole), SystemRole.Admin)!.ToLower() },
					{ nameof(Role.IsSystem).ToLower(), true }
				});
		}
	}
}