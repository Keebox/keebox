using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.Managers
{
	public class RoleManager : IRoleManager
	{
		public RoleManager(IRoleRepository roleRepository)
		{
			_roleRepository = roleRepository;
		}

		public IEnumerable<Role> GetRoles()
		{
			return _roleRepository.List();
		}

		public Guid CreateRole(string name)
		{
			return _roleRepository.Create(name);
		}

		public void UpdateRole(Role role)
		{
			_roleRepository.Update(role);
		}

		public void DeleteRole(Guid roleId)
		{
			_roleRepository.Delete(roleId);
		}

		private readonly IRoleRepository _roleRepository;
	}
}