using System;

using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.DataAccess.Repositories.InMemory;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DataAccess.Repositories
{
	public class InMemoryRepositoryContext : IRepositoryContext
	{
		private readonly IServiceProvider _serviceProvider;

		public InMemoryRepositoryContext(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ISecretRepository GetSecretRepository()
		{
			return _serviceProvider.GetRequiredService<InMemorySecretRepository>();
		}

		public IAccountRepository GetAccountRepository()
		{
			return _serviceProvider.GetRequiredService<InMemoryAccountRepository>();
		}

		public IGroupRepository GetGroupRepository()
		{
			return _serviceProvider.GetRequiredService<InMemoryGroupRepository>();
		}

		public IRoleRepository GetRoleRepository()
		{
			return _serviceProvider.GetRequiredService<InMemoryRoleRepository>();
		}

		public IPermissionRepository GetPermissionRepository()
		{
			return _serviceProvider.GetRequiredService<InMemoryPermissionRepository>();
		}
	}
}