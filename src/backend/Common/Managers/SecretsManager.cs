﻿using System;
using System.Collections.Generic;
using System.Linq;

using Ralfred.Common.DataAccess.Entities;
using Ralfred.Common.DataAccess.Repositories;
using Ralfred.Common.DataAccess.Repositories.Abstractions;
using Ralfred.Common.Exceptions;
using Ralfred.Common.Helpers;
using Ralfred.Common.Types;


namespace Ralfred.Common.Managers
{
	public class SecretsManager : ISecretsManager
	{
		private readonly IGroupRepository _groupRepository;

		private readonly IPathResolver _pathResolver;
		private readonly ISecretsRepository _secretsRepository;

		public SecretsManager(IPathResolver pathResolver, IRepositoryContext repositoryContext)
		{
			_pathResolver = pathResolver;

			_secretsRepository = repositoryContext.GetSecretRepository();
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
					var groupSecrets = _secretsRepository.GetGroupSecrets(group.Id);

					var secret = groupSecrets.FirstOrDefault(x => x.Name == name);

					if (secret is null)
						throw new NotFoundException("Group does not contain such secret");

					return new[] { secret };
				}
				case PathType.Group:
				{
					var (groupName, folderPath) = _pathResolver.DeconstructPath(path);
					var group = _groupRepository.Get(groupName, folderPath);

					return _secretsRepository.GetGroupSecrets(group.Id)
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

					_secretsRepository.SetGroupSecrets(groupId, FilterDictionaryKeys(input, secrets), FilterDictionaryKeys(files, secrets));

					break;
				}
				case PathType.Group:
				{
					var (groupName, folderPath) = _pathResolver.DeconstructPath(path);
					var group = _groupRepository.Get(groupName, folderPath);

					if (secrets.Length > 0)
						_secretsRepository.UpdateGroupSecrets(@group.Id, FilterDictionaryKeys(input, secrets),
							FilterDictionaryKeys(files, secrets));
					else
						_secretsRepository.SetGroupSecrets(@group.Id, input, files);

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
						_secretsRepository.UpdateGroupSecrets(@group.Id, new Dictionary<string, string> { { name, input["value"] } },
							new Dictionary<string, string>());
					else
						_secretsRepository.UpdateGroupSecrets(@group.Id, new Dictionary<string, string>(),
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

					_secretsRepository.DeleteGroupSecrets(group.Id, new[] { secretName });

					break;
				}
				case PathType.Group:
				{
					var (groupName, groupPath) = _pathResolver.DeconstructPath(path);

					var group = _groupRepository.Get(groupName, groupPath);

					if (secrets.Length > 0)
						_secretsRepository.DeleteGroupSecrets(@group.Id, secrets);
					else
					{
						_groupRepository.DeleteGroup(groupName, groupPath);
						_secretsRepository.DeleteGroupSecrets(group.Id);
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