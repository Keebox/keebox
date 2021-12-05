using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.Models.EntityCreation;
using Keebox.SecretsService.RequestFiltering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate] [AuthorizePrivileged]
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
		public ActionResult<Guid> CreatePermission([FromBody] PermissionCreationPayload payload)
		{
			var (roleId, groupId, isReadOnly) = ParsePermission(payload);

			return Ok(_permissionManager.CreatePermission(roleId, groupId, isReadOnly));
		}

		[HttpPut("{permissionId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult ReplacePermission([FromBody] Permission permission, [FromRoute] Guid permissionId)
		{
			if (permissionId != permission.Id)
			{
				throw new ArgumentException("Ids do not match.");
			}

			_permissionManager.UpdatePermission(permission);

			return NoContent();
		}

		[HttpDelete("{permissionId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeletePermission([FromRoute] Guid permissionId)
		{
			_permissionManager.DeletePermission(permissionId);

			return NoContent();
		}

		private static (Guid roleId, Guid groupId, bool isReadOnly) ParsePermission(PermissionCreationPayload payload)
		{
			if (payload.RoleId == null)
			{
				throw new ArgumentException("Role id is not provided.");
			}

			if (payload.GroupId == null)
			{
				throw new ArgumentException("Group id is not provided.");
			}

			if (payload.IsReadonly == null)
			{
				throw new ArgumentException("Is read only is not provided.");
			}

			return ((Guid roleId, Guid groupId, bool isReadOnly))(payload.RoleId, payload.GroupId, payload.IsReadonly);
		}

		private readonly IPermissionManager _permissionManager;
	}
}