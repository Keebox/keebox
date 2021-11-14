using System;

using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.DataAccess.Repositories.Postgres;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DataAccess.Repositories
{
	public class PostgresRepositoryContext : IRepositoryContext
	{
		private readonly IServiceProvider _serviceProvider;

		public PostgresRepositoryContext(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ISecretRepository GetSecretRepository()
		{
			return _serviceProvider.GetService<PostgresSecretRepository>()!;
		}

		public IAccountRepository GetAccountRepository()
		{
			return _serviceProvider.GetService<PostgresAccountRepository>()!;
		}

		public IGroupRepository GetGroupRepository()
		{
			return _serviceProvider.GetService<PostgresGroupRepository>()!;
		}

		public IRoleRepository GetRoleRepository()
		{
			return _serviceProvider.GetService<PostgresRoleRepository>()!;
		}
	}
}