using System;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.InMemory.Migrations.Runner;
using Keebox.Common.Types;


namespace Keebox.Common.DataAccess.Repositories.InMemory.Migrations;

public class SystemRolesMigration : InMemoryMigration
{
	protected override void Up()
	{
		UseStorageOf<Role>().Storage.Add(new Role
		{
			Id = Guid.NewGuid(),
			Name = FormattedSystemRole.Admin,
			IsSystem = true
		});
	}
}