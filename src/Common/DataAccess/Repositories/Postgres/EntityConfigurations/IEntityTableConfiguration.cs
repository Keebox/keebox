using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations
{
	public interface IEntityTableConfiguration
	{
		void Configure(MappingSchema schema);
	}
}