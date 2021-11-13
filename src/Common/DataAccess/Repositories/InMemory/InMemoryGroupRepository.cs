using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryGroupRepository : InMemoryRepositoryBase<Group>, IGroupRepository
	{
		public bool Exists(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			return Storage.Any(x =>
				x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
		}

		public Group Get(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			try
			{
				return Storage.Single(x =>
					x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
			}
			catch (InvalidOperationException)
			{
				throw new NotFoundException("Group not found");
			}
		}

		public Guid CreateGroup(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			var group = new Group
			{
				Id = Guid.NewGuid(),
				Name = name,
				Path = path
			};

			Storage.Add(group);

			return group.Id;
		}

		public void DeleteGroup(string name, string path)
		{
			EnsureArg.IsNotEmptyOrWhiteSpace(name);

			var items = Storage.Where(x =>
					x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
				.ToList();

			items.ForEach(x => Storage.Remove(x));
		}
	}
}