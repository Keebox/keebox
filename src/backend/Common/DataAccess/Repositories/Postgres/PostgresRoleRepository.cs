using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
    public class PostgresRoleRepository : IRolesRepository
    {
        public PostgresRoleRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private readonly IConnectionFactory _connectionFactory;
    }
}