using Keebox.Common.DataAccess.Entities;

using LinqToDB;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	public class PermissionEntityTableConfiguration : IEntityTableConfiguration
	{
		public void Configure(MappingSchema schema)
		{
			schema
				.GetFluentMappingBuilder()
				.Entity<Permission>()
				.HasSchemaName("public")
				.HasTableName("permission")
				.Property(x => x.Id).HasColumnName(nameof(Permission.Id).ToLower()).IsPrimaryKey().HasDataType(DataType.Guid)
				.Property(x => x.GroupId).HasColumnName(nameof(Permission.GroupId).ToLower()).HasDataType(DataType.Guid)
				.Property(x => x.RoleId).HasColumnName(nameof(Permission.RoleId).ToLower()).HasDataType(DataType.Guid)
				.Property(x => x.IsReadOnly).HasColumnName(nameof(Permission.IsReadOnly).ToLower()).HasDataType(DataType.Boolean);
		}
	}
}