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

		public Guid Create(string name)
		{
			EnsureArg.IsNotNullOrWhiteSpace(name);

			var role = new Role
			{
				Id = Guid.NewGuid(),
				Name = name
			};

			Storage.Add(role);

			return role.Id;
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

		public Role Get(Guid roleId)
		{
			return Storage.Single(x => x.Id == roleId);
		}

		public bool Exists(Guid roleId)
		{
			return Storage.Any(x => x.Id == roleId);
		}

		public bool Exists(string name)
		{
			return Storage.Any(x => x.Name.Equals(name));
		}
	}
}