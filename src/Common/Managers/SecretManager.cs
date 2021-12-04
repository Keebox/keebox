using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;


namespace Keebox.Common.Managers
{
	public class SecretManager : ISecretManager
	{
		private readonly IGroupRepository _groupRepository;

		private readonly IPathResolver _pathResolver;
		private readonly ISecretRepository _secretRepository;

		public SecretManager(IPathResolver pathResolver, IRepositoryContext repositoryContext)
		{
			_pathResolver = pathResolver;

			_secretRepository = repositoryContext.GetSecretRepository();
			_groupRepository = repositoryContext.GetGroupRepository();
		}

		private Dictionary<string, string> FilterDictionaryKeys(Dictionary<string, string> dictionary, string[] keys)
		{
			return keys.Any()
				? dictionary
					.Where(x => keys.Contains(x.Key))
					.ToDictionary(x => x.Key, x => x.Value)
				: dictionary;
		}

		public IEnumerable<Secret> GetSecrets(string path, string[] secrets)
		{
			var pathType = _pathResolver.Resolve(path);

			switch (pathType)
			{
				case PathType.Secret:
				{
					var (name, groupPath) = _pathResolver.DeconstructPath(path);
					var (groupName, folderPath) = _pathResolver.DeconstructPath(groupPath);

					var group = _groupRepository.Get(groupName, folderPath);
					var groupSecrets = _secretRepository.GetGroupSecrets(group.Id);

					var secret = groupSecrets.FirstOrDefault(x => x.Name == name);

					if (secret is null)
						throw new NotFoundException("Group does not contain such secret");

					return new[] { secret };
				}
				case PathType.Group:
				{
					var (groupName, folderPath) = _pathResolver.DeconstructPath(path);
					var group = _groupRepository.Get(groupName, folderPath);

					return _secretRepository.GetGroupSecrets(group.Id)
						.Where(x => secrets.Length == 0 || secrets.Contains(x.Name));
				}
				case PathType.None:
					throw new NotFoundException("Path not found");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void AddSecrets(string path, Dictionary<string, string> input, Dictionary<string, string> files, string[] secrets)
		{
			var pathType = _pathResolver.Resolve(path);

			switch (pathType)
			{
				case PathType.None:
				{
					var (groupName, folderPath) = _pathResolver.DeconstructPath(path);

					var groupId = _groupRepository.CreateGroup(groupName, folderPath);

					_secretRepository.SetGroupSecrets(groupId, FilterDictionaryKeys(input, secrets), FilterDictionaryKeys(files, secrets));

					break;
				}
				case PathType.Group:
				{
					var (groupName, folderPath) = _pathResolver.DeconstructPath(path);
					var group = _groupRepository.Get(groupName, folderPath);

					if (secrets.Length > 0)
						_secretRepository.UpdateGroupSecrets(group.Id, FilterDictionaryKeys(input, secrets),
							FilterDictionaryKeys(files, secrets));
					else
						_secretRepository.SetGroupSecrets(group.Id, input, files);

					break;
				}
				case PathType.Secret:
				{
					if (!input.ContainsKey("value") && !files.ContainsKey("value"))
						throw new ArgumentException("Value is not provided");

					var (name, groupPath) = _pathResolver.DeconstructPath(path);
					var (groupName, folderPath) = _pathResolver.DeconstructPath(groupPath);

					var group = _groupRepository.Get(groupName, folderPath);

					if (input.ContainsKey("value"))
						_secretRepository.UpdateGroupSecrets(group.Id, new Dictionary<string, string> { { name, input["value"] } },
							new Dictionary<string, string>());
					else
						_secretRepository.UpdateGroupSecrets(group.Id, new Dictionary<string, string>(),
							new Dictionary<string, string> { { name, files["value"] } });

					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DeleteSecrets(string path, string[] secrets)
		{
			var pathType = _pathResolver.Resolve(path);

			switch (pathType)
			{
				case PathType.Secret:
				{
					var (secretName, groupPath) = _pathResolver.DeconstructPath(path);
					var (groupName, folderPath) = _pathResolver.DeconstructPath(groupPath);

					var group = _groupRepository.Get(groupName, folderPath);

					_secretRepository.DeleteGroupSecrets(group.Id, new[] { secretName });

					break;
				}
				case PathType.Group:
				{
					var (groupName, groupPath) = _pathResolver.DeconstructPath(path);

					var group = _groupRepository.Get(groupName, groupPath);

					if (secrets.Length > 0)
						_secretRepository.DeleteGroupSecrets(group.Id, secrets);
					else
					{
						_groupRepository.DeleteGroup(groupName, groupPath);
						_secretRepository.DeleteGroupSecrets(group.Id);
					}

					break;
				}
				case PathType.None:
					throw new NotFoundException("Path not found");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}