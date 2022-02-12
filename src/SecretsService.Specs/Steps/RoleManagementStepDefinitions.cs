using System;

using AutoFixture;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class RoleManagementStepDefinitions
{
	public RoleManagementStepDefinitions(ScenarioContext scenarioContext, ApiRequestSender requestSender)
	{
		_scenarioContext = scenarioContext;
		_requestSender = requestSender;
	}

	[Then(@"roles should be returned")]
	public void ThenRolesShouldBeReturned()
	{
		var roles = _scenarioContext.GetResponseBody<Role[]>();

		roles.Should().NotBeNullOrEmpty();
	}

	[Given(@"a new created role")]
	public void GivenANewCreatedRole()
	{
		var roleName = $"auto-{Guid.NewGuid().ToString().ToLower()}";

		_requestSender.BecomeAdmin();
		var id = _requestSender.CreateRole(roleName, out _);

		_scenarioContext.Add(RoleNameKey, roleName);
		_scenarioContext.Add(RoleIdKey, JsonConvert.DeserializeObject<string>(id));
	}

	[Given(@"a new role with name (.*)")]
	public void GivenANewRoleWithName(string name)
	{
		_requestSender.BecomeAdmin();
		var id = _requestSender.CreateRole(name, out _);

		_scenarioContext.Add(RoleNameKey, name);
		_scenarioContext.Add(RoleIdKey, JsonConvert.DeserializeObject<string>(id));
	}

	[Given(@"a non existing role id")]
	public void GivenANonExistingRoleId()
	{
		_requestSender.BecomeAdmin();
		_scenarioContext.Add(RoleIdKey, _fixture.Create<string>());
	}

	[Then(@"role should be returned")]
	public void ThenRoleShouldBeReturned()
	{
		var role = _scenarioContext.GetResponseBody<Role>();

		role.Id.Should().Be(_scenarioContext.Get<string>(RoleIdKey));
		role.Name.Should().Be(_scenarioContext.Get<string>(RoleNameKey));
		role.IsSystem.Should().BeFalse();
	}

	[Given(@"a new role name")]
	public void GivenANewRoleName()
	{
		_scenarioContext.Set(_fixture.Create<string>(), RoleNameKey);
	}

	[Given(@"a role id")]
	public void GivenARoleId()
	{
		_scenarioContext.Set(_fixture.Create<string>(), RoleIdKey);
		_scenarioContext.Set(_fixture.Create<string>(), RoleNameKey);
	}

	[Given(@"same role name in body")]
	public void GivenSameRoleNameInBody()
	{
		var name = _scenarioContext.Get<string>(RoleNameKey);
		var request = _scenarioContext.GetRequest();
		request.AddJsonBody(new { Name = name });
	}

	[Given(@"role is in body")]
	public void GivenRoleIsInBody()
	{
		var roleId = _scenarioContext.Get<string>(RoleIdKey);
		var roleName = _scenarioContext.Get<string>(RoleNameKey);
		var request = _scenarioContext.GetRequest();
		request.AddJsonBody(new { Id = roleId, Name = roleName });
	}

	private readonly Fixture _fixture = new();

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;

	private const string RoleIdKey = "RoleId";
	private const string RoleNameKey = "RoleName";
}