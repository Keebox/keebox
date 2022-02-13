using System;
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
	[Route(RouteMap.Role)]
	[Authenticate, AuthorizePrivileged]
	[OpenApiTags("Privileged", "Role")]
	public class RoleController : ControllerBase
	{
		public RoleController(IRoleManager roleManager)
		{
			_roleManager = roleManager;
		}

		[HttpGet("{roleId:guid}")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(Role))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Get role by id", "Gets role by provided id")]
		public ActionResult<Role> GetRole([FromRoute] Guid roleId)
		{
			return Ok(_roleManager.GetRole(roleId));
		}

		[HttpGet]
		[SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Role>))]
		[OpenApiOperation("Get all roles", "Gets all created roles")]
		public ActionResult<IEnumerable<Role>> ListRoles()
		{
			return Ok(_roleManager.GetRoles());
		}

		[HttpPost]
		[SwaggerResponse(HttpStatusCode.Created, typeof(string))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.Conflict, typeof(Error))]
		[OpenApiOperation("Create role", "Creates role with specific name")]
		public ActionResult<string> CreateRole([FromBody] RoleCreationPayload payload)
		{
			var roleId = _roleManager.CreateRole(payload.Name ?? string.Empty);

			return Created($"role/{roleId}", roleId.ToString());
		}

		[HttpPut("{roleId:guid}")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Update role by id", "Provided role replaces existing role  with given id")]
		public ActionResult ReplaceRole([FromBody] Role role, [FromRoute] Guid roleId)
		{
			if (roleId != role.Id) throw new ArgumentException("Ids do not match");

			_roleManager.UpdateRole(role);

			return NoContent();
		}

		[HttpDelete("{roleId:guid}")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		[OpenApiOperation("Delete role by id", "Deletes role by provided id")]
		public ActionResult DeleteRole([FromRoute] Guid roleId)
		{
			_roleManager.DeleteRole(roleId);

			return NoContent();
		}

		private readonly IRoleManager _roleManager;
	}
}