using LinqToDB.Data;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public interface IConnectionFactory
	{
		DataConnection Create();
	}
}