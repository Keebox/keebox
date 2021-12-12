using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.Middlewares.Attributes;
using Keebox.SecretsService.Models.EntityCreation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<Role> GetRole([FromRoute] Guid roleId)
		{
			return Ok(_roleManager.GetRole(roleId));
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<Role>> ListRoles()
		{
			return Ok(_roleManager.GetRoles());
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public ActionResult<string> CreateRole([FromBody] RoleCreationPayload payload)
		{
			return Ok(_roleManager.CreateRole(payload.Name ?? string.Empty).ToString());
		}

		[HttpPut("{roleId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult ReplaceRole([FromBody] Role role, [FromRoute] Guid roleId)
		{
			if (roleId != role.Id) throw new ArgumentException("Ids do not match");

			_roleManager.UpdateRole(role);

			return NoContent();
		}

		[HttpDelete("{roleId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteRole([FromRoute] Guid roleId)
		{
			_roleManager.DeleteRole(roleId);

			return NoContent();
		}

		private readonly IRoleManager _roleManager;
	}
}