using System;

using AutoFixture;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class RoleStepDefinition
{
	public RoleStepDefinition(ScenarioContext scenarioContext, ApiRequestSender requestSender)
	{
		_scenarioContext = scenarioContext;
		_requestSender = requestSender;
	}

	[Then(@"roles should be returned")]
	public void ThenRolesShouldBeReturned()
	{
		var roles = _scenarioContext.GetBody<Role[]>();

		roles.Should().NotBeNullOrEmpty();
	}

	[Given(@"a new created role")]
	public void GivenANewCreatedRole()
	{
		var roleName = $"auto-{Guid.NewGuid().ToString().ToLower()}";

		_requestSender.BecomeAdmin();
		var id = _requestSender.CreateRole(roleName, out _);

		_scenarioContext.Add("RoleName", roleName);
		_scenarioContext.Add("RoleId", JsonConvert.DeserializeObject<string>(id));
	}

	[Given(@"a new role with name (.*)")]
	public void GivenANewRoleWithName(string name)
	{
		_requestSender.BecomeAdmin();
		var id = _requestSender.CreateRole(name, out _);

		_scenarioContext.Add("RoleName", name);
		_scenarioContext.Add("RoleId", JsonConvert.DeserializeObject<string>(id));
	}

	[Given(@"a non existing role id")]
	public void GivenANonExistingRoleId()
	{
		_requestSender.BecomeAdmin();
		_scenarioContext.Add("RoleId", _fixture.Create<string>());
	}

	[Then(@"role should be returned")]
	public void ThenRoleShouldBeReturned()
	{
		var role = _scenarioContext.GetBody<Role>();

		role.Id.Should().Be(_scenarioContext.Get<string>("RoleId"));
		role.Name.Should().Be(_scenarioContext.Get<string>("RoleName"));
		role.IsSystem.Should().BeFalse();
	}

	[Given(@"a new role name")]
	public void GivenANewRoleName()
	{
		_scenarioContext.Set(_fixture.Create<string>(), "RoleName");
	}

	[Given(@"a role id")]
	public void GivenARoleId()
	{
		_scenarioContext.Set(_fixture.Create<string>(), "RoleId");
		_scenarioContext.Set(_fixture.Create<string>(), "RoleName");
	}

	[Then(@"role should not exists")]
	public void ThenRoleShouldNotExists()
	{
		_requestSender.GetRole(_scenarioContext.Get<string>("RoleId"), out var response);

		response.StatusCode.Should().Be(404);
	}

	private readonly Fixture _fixture = new();

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;
}