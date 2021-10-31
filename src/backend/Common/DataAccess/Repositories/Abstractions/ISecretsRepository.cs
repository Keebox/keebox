using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.DataAccess.Repositories.Abstractions
{
	public interface ISecretsRepository
	{
		IEnumerable<Secret> GetGroupSecrets(Guid groupId);

		void UpdateGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files);

		void SetGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files);

		void DeleteGroupSecrets(Guid groupId, IEnumerable<string> secrets);

		void DeleteGroupSecrets(Guid groupId);
	}
}