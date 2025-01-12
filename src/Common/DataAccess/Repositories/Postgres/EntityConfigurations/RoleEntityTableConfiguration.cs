﻿using Keebox.Common.DataAccess.Entities;

using LinqToDB;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	internal sealed class RoleEntityTableConfiguration : IEntityTableConfiguration
	{
		public void Configure(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder().Entity<Role>()
				.HasSchemaName("public")
				.HasTableName("role")
				.Property(x => x.Id).HasColumnName(nameof(Role.Id).ToLower()).IsPrimaryKey().HasDataType(DataType.Guid)
				.Property(x => x.Name).HasColumnName(nameof(Role.Name).ToLower())
				.Property(x => x.IsSystem).HasColumnName(nameof(Role.IsSystem).ToLower()).HasDataType(DataType.Boolean);
		}
	}
}