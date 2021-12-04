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
			return _serviceProvider.GetRequiredService<PostgresSecretRepository>();
		}

		public IAccountRepository GetAccountRepository()
		{
			return _serviceProvider.GetRequiredService<PostgresAccountRepository>();
		}

		public IGroupRepository GetGroupRepository()
		{
			return _serviceProvider.GetRequiredService<PostgresGroupRepository>();
		}

		public IRoleRepository GetRoleRepository()
		{
			return _serviceProvider.GetRequiredService<PostgresRoleRepository>();
		}

		public IPermissionRepository GetPermissionRepository()
		{
			return _serviceProvider.GetRequiredService<PostgresPermissionRepository>();
		}

		public IAssignmentRepository GetAssignmentRepository()
		{
			return _serviceProvider.GetRequiredService<PostgresAssignmentRepository>();
		}
	}
}