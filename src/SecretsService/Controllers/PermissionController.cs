﻿using System;
using System.Collections.Generic;
using System.Net;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.Middlewares.Attributes;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Models.EntityCreation;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate, AuthorizePrivileged]
	[OpenApiTags("Privileged", "Permission")]
	[Route(RouteMap.Permission)]
	public class PermissionController : ControllerBase
	{
		public PermissionController(IPermissionManager permissionManager)
		{
			_permissionManager = permissionManager;
		}

		[HttpGet("group/{groupId:guid}")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Permission>))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Get group permissions by id", "Gets group permissions by provided id")]
		public ActionResult<IEnumerable<Permission>> GetGroupPermissions([FromRoute] Guid groupId)
		{
			return Ok(_permissionManager.GetGroupPermissions(groupId));
		}

		[HttpGet("{permissionId:guid}")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(Permission))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Get permission by id", "Gets permission by provided id")]
		public ActionResult<Permission> GetPermission([FromRoute] Guid permissionId)
		{
			return Ok(_permissionManager.GetPermission(permissionId));
		}

		[HttpPost]
		[SwaggerResponse(HttpStatusCode.Created, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.Conflict, typeof(Error))]
		[OpenApiOperation("Create permission", "Creates permission for provided group and role")]
		public ActionResult<string> CreatePermission([FromBody] PermissionCreationPayload payload)
		{
			var (roleId, groupId, isReadOnly) = ParsePermission(payload);
			var permissionId = _permissionManager.CreatePermission(roleId, groupId, isReadOnly);

			return Created($"/permission/{permissionId}", permissionId.ToString());
		}

		[HttpPut("{permissionId:guid}")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Update permission by id", "Provided permission replaces existing permission with given id")]
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
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Delete permission by id", "Deletes permission by provided id")]
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