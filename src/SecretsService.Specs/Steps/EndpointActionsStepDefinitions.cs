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

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;

	[When(@"request has been sent")]
	public void WhenRequestHasBeenSent()
	{
		var request = _scenarioContext.GetRequest();
		var response = _requestSender.SendRequest(request);
		_scenarioContext.SetResponse(response);
	}
}