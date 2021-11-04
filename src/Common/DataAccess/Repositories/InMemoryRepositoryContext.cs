using System;

using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.DataAccess.Repositories.InMemory;

using Microsoft.Extensions.DependencyInjection;


namespace Keebox.Common.DataAccess.Repositories
{
	public class InMemoryRepositoryContext : IRepositoryContext
	{
		private readonly IServiceProvider _serviceProvider;

		public InMemoryRepositoryContext(IServiceProvider serviceProvider) =>
			_serviceProvider = serviceProvider;

		public ISecretsRepository GetSecretRepository()
		{
			return _serviceProvider.GetService<InMemorySecretRepository>()!;
		}

		public IAccountRepository GetAccountRepository()
		{
			return _serviceProvider.GetService<InMemoryAccountRepository>()!;
		}

		public IGroupRepository GetGroupRepository()
		{
			return _serviceProvider.GetService<InMemoryGroupRepository>()!;
		}

		public IRolesRepository GetRoleRepository()
		{
			return _serviceProvider.GetService<InMemoryRoleRepository>()!;
		}
	}
}