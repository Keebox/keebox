using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class PostgresRoleRepository : IRolesRepository
	{
		private readonly IConnectionFactory _connectionFactory;

		public PostgresRoleRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}
	}
}