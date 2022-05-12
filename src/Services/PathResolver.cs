using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repository;

using Services.Extensions;


namespace Services;

public class PathResolver : IPathResolver
{
	private readonly IRepository<Group> _groupRepository;
	private readonly IRepository<Secret> _secretRepository;

	public PathResolver(IRepository<Group> groupRepository, IRepository<Secret> secretRepository)
	{
		_groupRepository = groupRepository;
		_secretRepository = secretRepository;
	}

	public PathType Resolve(string path)
	{
		if (_groupRepository.Queryable.Any(x => x.Path.Equals(path)))
		{
			return PathType.Group;
		}

		var (groupPath, name) = path.DeconstructPath();

		if (_secretRepository.Queryable.Any(x => x.Name.Equals(name) && x.Group.Path.Equals(groupPath)))
		{
			return PathType.Secret;
		}

		if (_groupRepository.Queryable.Any(x => path.StartsWith(x.Path)))
		{
			throw new ArgumentException("Part of path is already points to existing group");
		}

		return PathType.None;
	}
}