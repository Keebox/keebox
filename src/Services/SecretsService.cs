using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repository;

using Services.Extensions;


namespace Services;

public class SecretsService : ISecretsService
{
	private readonly IRepository<Group> _groupRepository;

	public SecretsService(IRepository<Group> groupRepository)
	{
		_groupRepository = groupRepository;
	}

	// public void Save(string path, Dictionary<string, string> secrets)
	// {
	// 	var (groupPath, groupName) = path.DeconstructPath();
	// 	var groupExists = _context.Groups.Any(x => x.Name.Equals(groupName) && x.Path.Equals(groupPath));
	//
	// 	if (groupExists)
	// 	{
	// 		var group = _context.Groups.Single(x => x.Name.Equals(groupName) && x.Path.Equals(groupPath));
	//
	// 		foreach (var secret in secrets)
	// 		{
	// 			var groupSecret = group.Secrets.SingleOrDefault(x => x.Name.Equals(secret.Key));
	// 			if (groupSecret is null)
	// 			{
	// 				var newSecret = new Secret
	// 				{
	// 					Name = secret.Key,
	// 					Value = secret.Value
	// 				};
	//
	// 				group.Secrets.Add(newSecret);
	// 			}
	// 			else
	// 			{
	// 				groupSecret.Value = secret.Value;
	// 			}
	// 		}
	//
	// 		var secretNames = secrets.Keys.Concat(files.Keys);
	//
	// 		var deletedSecrets = group.Secrets.Where(x => !secretNames.Contains(x.Name));
	//
	// 		foreach (var secret in deletedSecrets)
	// 		{
	// 			_context.Remove(secret);
	// 		}
	// 	}
	// 	else
	// 	{
	// 		var group = new Group
	// 		{
	// 			Name = groupName,
	// 			Path = groupPath,
	// 			Secrets = secrets.Select(x => new Secret
	// 			{
	// 				Name = x.Key,
	// 				Value = x.Value
	// 			}).Concat(files.Select(x => new Secret
	// 			{
	// 				Name = x.Key,
	// 				File = x.Value
	// 			})).ToArray()
	// 		};
	//
	// 		_context.Groups.Add(group);
	// 	}
	//
	// 	_context.SaveChanges();
	// }
	//
	// public void Save(string path, string secret)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public void Save(string path, byte[] file)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public object Get(string path)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public void Delete(string path)
	// {
	// 	throw new NotImplementedException();
	// }
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
		var group = _groupRepository.Queryable.Single(x => x.Path.Equals(path));

		group.Secrets = secrets.Select(x => new Secret { Name = x.Key, Value = x.Value }).ToArray();

		_groupRepository.Update(group);
	}
}