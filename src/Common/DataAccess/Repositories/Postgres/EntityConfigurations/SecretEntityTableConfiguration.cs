using Keebox.Common.DataAccess.Entities;

using LinqToDB;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	internal sealed class SecretEntityTableConfiguration : IEntityTableConfiguration
	{
		public void Configure(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder()
				.Entity<Secret>()
				.HasSchemaName("public")
				.HasTableName("secret")
				.Property(x => x.Id).HasColumnName(nameof(Secret.Id).ToLower()).IsPrimaryKey().HasDataType(DataType.Guid)
				.Property(x => x.Name).HasColumnName(nameof(Secret.Name).ToLower())
				.Property(x => x.Value).HasColumnName(nameof(Secret.Value).ToLower())
				.Property(x => x.GroupId).HasColumnName(nameof(Secret.GroupId).ToLower()).HasDataType(DataType.Guid)
				.Property(x => x.IsFile).HasColumnName(nameof(Secret.IsFile).ToLower());
		}
	}
}