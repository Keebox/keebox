using System;
using System.Collections.Generic;
using System.Net;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.RequestFiltering;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Authenticate]
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
		public Role GetRole([FromQuery] Guid roleId)
		{
			return _roleManager.GetRole(roleId);
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IEnumerable<Role> ListRoles()
		{
			return _roleManager.GetRoles();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public Guid CreateRole([FromBody] string name)
		{
			return _roleManager.CreateRole(name);
		}

		[HttpPut("{roleId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public void ReplaceRole([FromBody] Role role, [FromQuery] Guid roleId)
		{
			if (roleId != role.Id)
			{
				throw new ArgumentException("Ids do not match");
			}

			_roleManager.UpdateRole(role);
		}

		[HttpDelete("{roleId:guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public void DeleteRole([FromQuery] Guid roleId)
		{
			_roleManager.DeleteRole(roleId);
		}

		private readonly IRoleManager _roleManager;
	}
}