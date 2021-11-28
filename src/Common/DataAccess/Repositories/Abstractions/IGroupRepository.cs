using System;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface IGroupRepository
	{
		bool Exists(string name, string path);

		bool Exists(Guid groupId);

		Group Get(string name, string path);

		Guid CreateGroup(string name, string path);

		void DeleteGroup(string name, string path);
	}
}