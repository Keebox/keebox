using System;
using System.Collections.Generic;
using System.Net;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.Middlewares.Attributes;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Models.EntityCreation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate] [AuthorizePrivileged]
	[Route(RouteMap.Role)]
	public class RoleController : ControllerBase
	{
		public RoleController(IRoleManager roleManager)
		{
			_roleManager = roleManager;
		}

		[HttpGet("{roleId:guid}")]
		[OpenApiOperation("Get role by id")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(Role))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult<Role> GetRole([FromRoute] Guid roleId)
		{
			return Ok(_roleManager.GetRole(roleId));
		}

		[HttpGet]
		[OpenApiOperation("Get all roles")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Role>))]
		public ActionResult<IEnumerable<Role>> ListRoles()
		{
			return Ok(_roleManager.GetRoles());
		}

		[HttpPost]
		[OpenApiOperation("Create role")]
		[SwaggerResponse(HttpStatusCode.Created, typeof(string))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.Conflict, typeof(Error))]
		public ActionResult<string> CreateRole([FromBody] RoleCreationPayload payload)
		{
			var roleId = _roleManager.CreateRole(payload.Name ?? string.Empty);

			return Created($"role/{roleId}", roleId.ToString());
		}

		[HttpPut("{roleId:guid}")]
		[OpenApiOperation("Update role by id", "Provided role replaces existing role  with given id")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult ReplaceRole([FromBody] Role role, [FromRoute] Guid roleId)
		{
			if (roleId != role.Id) throw new ArgumentException("Ids do not match");

			_roleManager.UpdateRole(role);

			return NoContent();
		}

		[HttpDelete("{roleId:guid}")]
		[OpenApiOperation("Delete role by id")]
		[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
		[SwaggerResponse(HttpStatusCode.NotFound, typeof(Error))]
		public ActionResult DeleteRole([FromRoute] Guid roleId)
		{
			_roleManager.DeleteRole(roleId);

			return NoContent();
		}

		private readonly IRoleManager _roleManager;
	}
}