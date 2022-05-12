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
		var (path2, name) = path.DeconstructPath();

		if (_groupRepository.Queryable.Any(x => x.Path.Equals(path2)))
		{
			return PathType.Group;
		}

		if (_groupRepository.Queryable.Any(x => path2.StartsWith(x.Path)))
		{
			throw new ArgumentException("Part of path is already points to existing group");
		}

		var (path3, name2) = path2.DeconstructPath();

		if (_secretRepository.Queryable.Any(x => x.Name.Equals(name) && x.Group.Path.Equals(path3)))
		{
			return PathType.Secret;
		}

		return PathType.None;
	}
}