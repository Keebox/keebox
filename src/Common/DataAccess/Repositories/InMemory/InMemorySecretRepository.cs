using System;
using System.Collections.Generic;
using System.Linq;

using EnsureThat;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories.InMemory
{
	public class InMemorySecretRepository : ISecretsRepository
	{
		public InMemorySecretRepository()
		{
			_storage = new List<Secret>();
		}

		private void UpdateSecret(Secret secret)
		{
			var index = _storage.FindIndex(x => x.Id == secret.Id);

			if (index == -1)
				return;

			_storage[index] = secret;
		}

		private void DeleteSecretByGroupId(Guid groupId)
		{
			var items = _storage.Where(x => x.GroupId.Equals(groupId)).ToList();
			items.ForEach(x => _storage.Remove(x));
		}

		private void DeleteSecretByGroupIdAndName(Guid groupId, string secret)
		{
			var items = _storage
				.Where(x => x.GroupId.Equals(groupId) && x.Name.Equals(secret, StringComparison.OrdinalIgnoreCase))
				.ToList();

			items.ForEach(x => _storage.Remove(x));
		}

		public IEnumerable<Secret> GetGroupSecrets(Guid groupId)
		{
			EnsureArg.IsNotDefault(groupId);

			return _storage.Where(x => x.GroupId.Equals(groupId));
		}

		public void UpdateGroupSecrets(Guid groupId, Dictionary<string, string> secrets, Dictionary<string, string> files)
		{
			EnsureArg.IsNotDefault(groupId);
			EnsureArg.IsNotNull(secrets);
			EnsureArg.IsNotNull(files);

			foreach (var (key, value) in secrets)
			{
				var secret = _storage.Single(x => x.GroupId == groupId && x.Name == key);
				secret.Value = value;

				UpdateSecret(secret);
			}

			foreach (var (key, value) in files)
			{
				var secret = _storage.Single(x => x.GroupId == groupId && x.Name == key);
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
				_storage.Add(new Secret
				{
					Name = key,
					Value = value,
					GroupId = groupId,
					Id = Guid.NewGuid(),
					IsFile = false
				});

			foreach (var (key, value) in files)
				_storage.Add(new Secret
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

		private readonly List<Secret> _storage;
	}
}