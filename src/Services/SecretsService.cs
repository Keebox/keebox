using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repository;

using Microsoft.EntityFrameworkCore;

using Services.Extensions;


namespace Services;

public class SecretsService : ISecretsService
{
	private readonly IRepository<Group> _groupRepository;
	private readonly IRepository<Secret> _secretRepository;

	public SecretsService(IRepository<Group> groupRepository, IRepository<Secret> secretRepository)
	{
		_groupRepository = groupRepository;
		_secretRepository = secretRepository;
	}

	public void CreateGroup(string path, Dictionary<string, string> secrets)
	{
		var group = new Group
		{
			Path = path,
			Secrets = secrets.Select(x => new Secret { Name = x.Key, Value = x.Value }).ToArray()
		};

		_groupRepository.Insert(group);
	}

	public void UpdateGroup(string path, Dictionary<string, string> secrets)
	{
		var group = _groupRepository.Queryable.Include(x => x.Secrets).Single(x => x.Path.Equals(path));

		var deletedSecrets = group.Secrets.Where(x => !secrets.ContainsKey(x.Name));

		foreach (var secret in deletedSecrets)
		{
			_secretRepository.Delete(secret.Id);
		}

		var secretsLookup = group.Secrets.ToDictionary(x => x.Name, x => x);

		group.Secrets = secrets.Select(x =>
		{
			if (!secretsLookup.TryGetValue(x.Key, out var secret))
			{
				return new Secret { Name = x.Key, Value = x.Value };
			}

			secret.Value = x.Value;

			return secret;
		}).ToArray();

		_groupRepository.Update(group);
	}

	public Secret[] GetGroupSecrets(string path)
	{
		var group = _groupRepository.Queryable.Include(x => x.Secrets).Single(x => x.Path.Equals(path));

		return group.Secrets.ToArray();
	}

	public Secret GetSecret(string path)
	{
		var (groupPath, name) = path.DeconstructPath();

		return _secretRepository.Queryable.Single(x => x.Name.Equals(name) && x.Group.Path.Equals(groupPath));
	}
}