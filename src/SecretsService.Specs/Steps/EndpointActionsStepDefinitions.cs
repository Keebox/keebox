using System;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class EndpointActionsStepDefinitions
{
	public EndpointActionsStepDefinitions(ScenarioContext scenarioContext, ApiRequestSender requestSender)
	{
		_scenarioContext = scenarioContext;
		_requestSender = requestSender;
	}

	[When(@"the login request has been sent")]
	public void WhenTheLoginRequestHasBeenSent()
	{
		var body = _requestSender.EraseToken().Login(_scenarioContext.Get<string>("AccountToken"), out var response);
		ParseResponse<string>(body, response);
	}

	[When(@"the get all roles request has been sent")]
	public void WhenTheGetAllRolesRequestHasBeenSent()
	{
		var body = _requestSender.GetAllRoles(out var response);
		ParseResponse<Role[]>(body, response);
	}

	[When(@"a get role request by id has been sent")]
	public void WhenAGetRoleRequestByIdHasBeenSent()
	{
		var body = _requestSender.GetRole(_scenarioContext.Get<string>("RoleId"), out var response);
		ParseResponse<Role>(body, response);
	}

	[When(@"an update role request has been sent")]
	public void WhenAnUpdateRoleRequestHasBeenSent()
	{
		var body = _requestSender.UpdateRole(new Role
		{
			Id = Guid.Parse(_scenarioContext.Get<string>("RoleId")),
			Name = _scenarioContext.Get<string>("RoleName")
		}, out var response);

		ParseResponse<object>(body, response);
	}

	[When(@"a create role request with the same name has been sent")]
	public void WhenACreateRoleRequestWithTheSameNameHasBeenSent()
	{
		var body = _requestSender.CreateRole(_scenarioContext.Get<string>("RoleName"), out var response);
		ParseResponse<string>(body, response);
	}

	[When(@"a delete request has been sent")]
	public void WhenADeleteRequestHasBeenSent()
	{
		var body = _requestSender.DeleteRole(_scenarioContext.Get<string>("RoleId"), out var response);
		ParseResponse<object>(body, response);
	}

	[Then(@"the status code should be (.*)")]
	public void ThenTheStatusCodeShouldBe(int expectedStatusCode)
	{
		var response = _scenarioContext.GetResponse();
		response.StatusCode.Should().Be(expectedStatusCode);
	}

	[Then(@"the message of error should be '(.*)'")]
	public void ThenTheMessageOfErrorShouldBe(string errorMessage)
	{
		var error = _scenarioContext.GetError();
		error.Message.Should().Be(errorMessage);
	}

	[Then(@"the status code in message should be (.*)")]
	public void ThenTheStatusCodeInMessageShouldBe(int code)
	{
		var error = _scenarioContext.GetError();
		error.StatusCode.Should().Be(code);
	}

	private void ParseResponse<T>(string body, IRestResponse response)
	{
		if (response.IsSuccessful)
		{
			_scenarioContext.SetBody<T>(body);
		}
		else
		{
			_scenarioContext.SetError(body);
		}

		_scenarioContext.SetResponse(response);
	}

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;
}