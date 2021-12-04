using Keebox.Common.DataAccess.Entities;

using LinqToDB;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	internal sealed class AccountEntityTableConfiguration : IEntityTableConfiguration
	{
		public void Configure(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder()
				.Entity<Account>()
				.HasSchemaName("public")
				.HasTableName("account")
				.Property(x => x.Id).HasColumnName(nameof(Account.Id).ToLower()).IsPrimaryKey().HasDataType(DataType.Guid)
				.Property(x => x.Name).HasColumnName(nameof(Account.Name).ToLower()).IsNullable()
				.Property(x => x.TokenHash).HasColumnName(nameof(Account.TokenHash).ToLower()).IsNullable()
				.Property(x => x.CertificateThumbprint).HasColumnName(nameof(Account.CertificateThumbprint).ToLower()).IsNullable();
		}
	}
}