using System;
using System.Collections.Generic;
using System.Linq;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.Exceptions;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.RequestFiltering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate]
	[Route(RouteMap.Permission)]
	public class PermissionController : ControllerBase
	{
		public PermissionController(IPermissionManager permissionManager)
		{
			_permissionManager = permissionManager;
		}

		[HttpGet("group/{groupId:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<IEnumerable<Permission>> GetGroupPermissions([FromRoute] Guid groupId)
		{
			return Ok(_permissionManager.GetGroupPermissions(groupId));
		}

		[HttpGet("{permissionId:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<Permission> GetPermission([FromRoute] Guid permissionId)
		{
			return Ok(_permissionManager.GetPermission(permissionId));
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public ActionResult<Guid> CreatePermission([FromRoute] RequestPayload payload)
		{
			if (payload.Data is null || !payload.Data.Keys.Any())
			{
				throw new EmptyDataException("Data is not provided");
			}

			var (roleId, groupId, isReadOnly) = ParsePermission(payload.Body!);

			return Ok(_permissionManager.CreatePermission(roleId, groupId, isReadOnly));
		}

		[HttpPut("permissionId:guid")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult ReplacePermission([FromBody] Permission permission, [FromRoute] Guid permissionId)
		{
			if (permissionId != permission.Id)
			{
				throw new ArgumentException("Ids do not match");
			}

			_permissionManager.UpdatePermission(permission);

			return NoContent();
		}

		[HttpDelete("permissionId:guid")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeletePermission([FromRoute] Guid permissionId)
		{
			_permissionManager.DeletePermission(permissionId);

			return NoContent();
		}

		private static (Guid roleId, Guid groupId, bool isReadOnly) ParsePermission(IReadOnlyDictionary<string, object> data)
		{
			if (!data.TryGetValue("roleId", out var roleId))
			{
				throw new ArgumentException("Role id is not provided");
			}

			if (!data.TryGetValue("groupId", out var groupId))
			{
				throw new ArgumentException("Group id is not provided");
			}

			if (!data.TryGetValue("isReadOnly", out var isReadOnly))
			{
				throw new ArgumentException("Is read only is not provided");
			}

			return (Guid.Parse((string)roleId), Guid.Parse((string)groupId), (bool)isReadOnly);
		}

		private readonly IPermissionManager _permissionManager;
	}
}