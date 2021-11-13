using System;
using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Managers;
using Keebox.SecretsService.RequestFiltering;

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
		public Role GetRole([FromQuery] Guid roleId)
		{
			return _roleManager.GetRole(roleId);
		}
		
		[HttpGet]
		public IEnumerable<Role> ListRoles()
		{
			return _roleManager.GetRoles();
		}

		[HttpPost]
		public Guid CreateRole([FromBody] string name)
		{
			return _roleManager.CreateRole(name);
		}

		[HttpPut("{roleId:guid}")]
		public void ReplaceRole([FromBody] Role role)
		{
			_roleManager.UpdateRole(role);
		}

		[HttpDelete("{roleId:guid}")]
		public void DeleteRole([FromQuery] Guid roleId)
		{
			_roleManager.DeleteRole(roleId);
		}

		private readonly IRoleManager _roleManager;
	}
}