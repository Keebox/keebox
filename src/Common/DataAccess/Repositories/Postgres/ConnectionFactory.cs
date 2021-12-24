using System;
using System.Linq;

using Keebox.Common.DataAccess.Repositories.Postgres.EntityConfigurations;
using Keebox.Common.Types;

using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;


namespace Keebox.Common.DataAccess.Repositories.Postgres
{
	public class ConnectionFactory : IConnectionFactory
	{
		public ConnectionFactory(StorageConnection storageConnection)
		{
			_storageConnection = storageConnection;
			_schema = new MappingSchema();

			var configurations = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
				.Where(x => typeof(IEntityTableConfiguration).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

			foreach (var configuration in configurations)
			{
				((IEntityTableConfiguration)Activator.CreateInstance(configuration)!).Configure(_schema);
			}
		}

		public DataConnection Create()
		{
			var connection = new DataConnection(new LinqToDbConnectionOptions(
					new LinqToDbConnectionOptionsBuilder()
						.UseConnectionString(new PostgreSQLDataProvider(), _storageConnection.ConnectionString)
						.UseMappingSchema(_schema)
				)
			);

			connection.RetryPolicy = new SqlServerRetryPolicy(10, TimeSpan.FromSeconds(5), null);

			return connection;
		}

		private readonly MappingSchema _schema;
		private readonly StorageConnection _storageConnection;
	}
}