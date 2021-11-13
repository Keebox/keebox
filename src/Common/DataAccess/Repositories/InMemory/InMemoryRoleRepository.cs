using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryRoleRepository : InMemoryRepositoryBase<Role>, IRoleRepository
	{
		public IEnumerable<Role> List()
		{
			return Storage;
		}

		public Role Create(Role role)
		{
			EnsureArg.IsNotNullOrWhiteSpace(role.Name);

			if (Storage.Any(x => x.Name == role.Name || x.Id == role.Id))
			{
				throw new AlreadyExistsException("Role already exists");
			}

			Storage.Add(role);

			return role;
		}

		public void Update(Role role)
		{
			EnsureArg.IsNotNullOrWhiteSpace(role.Name);

			var index = Storage.FindIndex(x => x.Id == role.Id);

			if (index == -1)
			{
				return;
			}

			Storage[index] = role;
		}

		public void Delete(Guid roleId)
		{
			Storage.RemoveAll(x => x.Id == roleId);
		}
	}
}