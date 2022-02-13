using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;


namespace Keebox.Common.Managers
{
	public class RoleManager : IRoleManager
	{
		public RoleManager(IRepositoryContext repositoryContext)
		{
			_roleRepository = repositoryContext.GetRoleRepository();
		}

		public IEnumerable<Role> GetRoles()
		{
			return _roleRepository.List();
		}

		public Guid CreateRole(string name)
		{
			if (_roleRepository.Exists(name))
			{
				throw new AlreadyExistsException("Role already exists.");
			}

			return _roleRepository.Create(name);
		}

		public void UpdateRole(Role role)
		{
			EnsureRole(role.Id);

			_roleRepository.Update(role);
		}

		public void DeleteRole(Guid roleId)
		{
			EnsureRole(roleId);

			_roleRepository.Delete(roleId);
		}

		public Role GetRole(Guid roleId)
		{
			EnsureRole(roleId);

			return _roleRepository.Get(roleId);
		}

		private void EnsureRole(Guid roleId)
		{
			if (!_roleRepository.Exists(roleId))
			{
				throw new NotFoundException("Role not found.");
			}
		}

		private readonly IRoleRepository _roleRepository;
	}
}