using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemorySecretRepository : InMemoryRepositoryBase<Secret>, ISecretsRepository
	{
		private void UpdateSecret(Secret secret)
		{
			var index = Storage.FindIndex(x => x.Id == secret.Id);

			if (index == -1)
				return;

			Storage[index] = secret;
		}

		private void DeleteSecretByGroupId(Guid groupId)
		{
			var items = Storage.Where(x => x.GroupId.Equals(groupId)).ToList();
			items.ForEach(x => Storage.Remove(x));
		}

		private void DeleteSecretByGroupIdAndName(Guid groupId, string secret)
		{
			var items = Storage
				.Where(x => x.GroupId.Equals(groupId) && x.Name.Equals(secret, StringComparison.OrdinalIgnoreCase))
				.ToList();

			items.ForEach(x => Storage.Remove(x));
		}

		public IEnumerable<Secret> GetGroupSecrets(Guid groupId)
		{
			EnsureArg.IsNotDefault(groupId);

			return Storage.Where(x => x.GroupId.Equals(groupId));
		}

		public void UpdateGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files)
		{
			EnsureArg.IsNotDefault(groupId);
			EnsureArg.IsNotNull(secrets);
			EnsureArg.IsNotNull(files);

			foreach (var (key, value) in secrets)
			{
				var secret = Storage.Single(x => x.GroupId == groupId && x.Name == key);
				secret.Value = value;

				UpdateSecret(secret);
			}

			foreach (var (key, value) in files)
			{
				var secret = Storage.Single(x => x.GroupId == groupId && x.Name == key);
				secret.Value = value;

				UpdateSecret(secret);
			}
		}

		public void SetGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files)
		{
			EnsureArg.IsNotDefault(groupId);
			EnsureArg.IsNotNull(secrets);
			EnsureArg.IsNotNull(files);

			DeleteSecretByGroupId(groupId);

			foreach (var (key, value) in secrets)
				Storage.Add(new Secret
				{
					Name = key,
					Value = value,
					GroupId = groupId,
					Id = Guid.NewGuid(),
					IsFile = false
				});

			foreach (var (key, value) in files)
				Storage.Add(new Secret
				{
					Name = key,
					Value = value,
					GroupId = groupId,
					Id = Guid.NewGuid(),
					IsFile = true
				});
		}

		public void DeleteGroupSecrets(Guid groupId, IEnumerable<string> secrets)
		{
			EnsureArg.IsNotDefault(groupId);

			foreach (var secret in secrets)
				DeleteSecretByGroupIdAndName(groupId, secret);
		}

		public void DeleteGroupSecrets(Guid groupId)
		{
			DeleteSecretByGroupId(groupId);
		}
	}
}