using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemoryRepositoryBase<T> where T : Entity
	{
		protected InMemoryRepositoryBase()
		{
			Storage = new List<T>();
		}

		protected readonly List<T> Storage;
	}
}