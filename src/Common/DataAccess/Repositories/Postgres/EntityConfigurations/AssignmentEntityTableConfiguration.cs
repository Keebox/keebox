using Keebox.Common.DataAccess.Entities;

using LinqToDB;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	public class AssignmentEntityTableConfiguration : IEntityTableConfiguration
	{
		public void Configure(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder()
				.Entity<Assignment>()
				.HasSchemaName("public")
				.HasTableName("assignment")
				.Property(x => x.Id).HasColumnName(nameof(Assignment.Id).ToLower()).IsPrimaryKey().HasDataType(DataType.Guid)
				.Property(x => x.AccountId).HasColumnName(nameof(Assignment.AccountId).ToLower()).HasDataType(DataType.Guid)
				.Property(x => x.RoleId).HasColumnName(nameof(Assignment.RoleId).ToLower()).HasDataType(DataType.Guid);
		}
	}
}