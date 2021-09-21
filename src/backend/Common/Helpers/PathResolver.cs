﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

using Ralfred.Common.DataAccess.Repositories;
using Ralfred.Common.Types;


namespace Ralfred.Common.Helpers
{
	public class PathResolver : IPathResolver
	{
		public PathResolver(IGroupRepository groupRepository)
		{
			_groupRepository = groupRepository;
		}

		#region Implementation of IPathResolver

		public PathType Resolve(string path)
		{
			if (!ValidatePath(path))
			{
				throw new ArgumentException("Path is not valid");
			}

			var (name, path2) = DeconstructPath(path);

			if (_groupRepository.Exists(name, path2))
			{
				return PathType.Group;
			}

			var (name2, path3) = DeconstructPath(path2);

			if (_groupRepository.Exists(name2, path3))
			{
				return PathType.Secret;
			}

			return PathType.None;
		}

		public (string name, string path) DeconstructPath(string fullPath)
		{
			const char separator = '/';

			var splitted = fullPath.Split(separator);

			var name = splitted.Last();
			var path = string.Join(separator, splitted.Take(splitted.Length - 1));

			return (name, path);
		}

		public bool ValidatePath(string path)
		{
			var pattern = new Regex(@"^[a-zA-Z0-9\-_]+(\/[a-zA-Z0-9\-_]+)*$");

			return pattern.IsMatch(path);
		}

		#endregion

		private readonly IGroupRepository _groupRepository;
	}
}