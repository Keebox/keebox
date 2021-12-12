using System;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.Exceptions;
using Keebox.Common.Helpers;
using Keebox.Common.Types;
using Keebox.SecretsService.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace Keebox.SecretsService.Middlewares
{
	public class OrdinaryAccessAuthorizationFilter : IActionFilter
	{
		public OrdinaryAccessAuthorizationFilter(RoleAccessStrategy roleAccessStrategy)
		{
			_roleAccessStrategy = roleAccessStrategy;
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var user = context.HttpContext.User;

			if (user.IsInRole(FormattedSystemRole.Admin)) return;

			var userRoleIds = user.GetNonSystemRoles().ToArray();
			var serviceProvider = context.HttpContext.RequestServices;

			var pathResolver = serviceProvider.GetRequiredService<IPathResolver>();
			var repositoryContext = serviceProvider.GetRequiredService<IRepositoryContext>();
			var groupRepository = repositoryContext.GetGroupRepository();
			var permissionRepository = repositoryContext.GetPermissionRepository();

			var path = context.HttpContext.Request.Path.ToString();
			var formattedPath = path.Remove(path.IndexOf('/'), 1);

			var pathType = pathResolver.Resolve(formattedPath);

			Guid groupId;

			switch (pathType)
			{
				case PathType.Secret:
				{
					var (_, groupPath) = pathResolver.DeconstructPath(formattedPath);
					var (groupName, folderPath) = pathResolver.DeconstructPath(groupPath);

					groupId = groupRepository.Get(groupName, folderPath).Id;

					break;
				}

				case PathType.Group:
				{
					var (groupName, folderPath) = pathResolver.DeconstructPath(formattedPath);
					groupId = groupRepository.Get(groupName, folderPath).Id;

					break;
				}

				case PathType.None:
					return;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var groupPermissions = permissionRepository.GetByGroupId(groupId).ToArray();
			var isReadonlyRequest = IsReadonlyRequest(context.HttpContext.Request.Method);

			if (!groupPermissions.Any()) return;

			var isAuthorized = _roleAccessStrategy switch
			{
				RoleAccessStrategy.All => ValidateStrictAccessPermissions(groupPermissions, userRoleIds, isReadonlyRequest),
				RoleAccessStrategy.Any => ValidateLenientAccessPermissions(groupPermissions, userRoleIds, isReadonlyRequest),

				_ => throw new ArgumentOutOfRangeException()
			};

			if (!isAuthorized) throw new UnauthorizedException("You don't have permissions to perform this action.");
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		private static bool ValidateStrictAccessPermissions(Permission[] groupPermissions, Guid[] userRoles, bool isReadonlyRequest)
		{
			var intersection = groupPermissions.Where(gp => userRoles.Contains(gp.RoleId)).ToList();

			if (!intersection.Any()) return false;

			return groupPermissions.Length == intersection.Count && (isReadonlyRequest || intersection.Any(x => !x.IsReadOnly));
		}

		private static bool ValidateLenientAccessPermissions(Permission[] groupPermissions, Guid[] userRoles, bool isReadonlyRequest)
		{
			var intersection = groupPermissions.Where(gp => userRoles.Contains(gp.RoleId)).ToList();

			if (!intersection.Any() && groupPermissions.Any()) return false;

			return isReadonlyRequest || intersection.Any(x => !x.IsReadOnly);
		}

		private static bool IsReadonlyRequest(string requestMethod)
		{
			return requestMethod.ToUpper().Equals(HttpMethods.Get);
		}

		private readonly RoleAccessStrategy _roleAccessStrategy;
	}
}