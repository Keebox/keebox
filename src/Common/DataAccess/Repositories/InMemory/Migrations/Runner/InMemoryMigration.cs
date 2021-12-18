using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.InMemory.Migrations.Runner;

public class InMemoryMigration
{
	protected InMemoryRepositoryBase<T> UseStorageOf<T>() where T : Entity
	{
		return ServiceProvider?.GetService(_inMemoryContextMap[typeof(T)]) as InMemoryRepositoryBase<T>
			   ?? throw new InvalidOperationException();
	}

	public IServiceProvider? ServiceProvider { get; set; }

	protected virtual void Up() { }

	private static Dictionary<Type, Type> _inMemoryContextMap = new()
	{
		{ typeof(Group), typeof(InMemoryGroupRepository) },
		{ typeof(Role), typeof(InMemoryRoleRepository) },
		{ typeof(Account), typeof(InMemoryAccountRepository) },
		{ typeof(Assignment), typeof(InMemoryAssignmentRepository) },
		{ typeof(Permission), typeof(InMemoryPermissionRepository) },
		{ typeof(Secret), typeof(InMemorySecretRepository) },
	};
}