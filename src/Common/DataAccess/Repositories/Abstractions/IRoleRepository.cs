using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IRoleRepository
	{
		IEnumerable<Role> List();

		Guid Create(string name);

		void Update(Role role);

		void Delete(Guid roleId);

		Role Get(Guid roleId);

		bool Exists(Guid roleId);

		bool Exists(string name);
	}
}