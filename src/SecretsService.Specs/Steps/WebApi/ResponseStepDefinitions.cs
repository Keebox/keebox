using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps.WebApi;

[Binding]
public class ResponseStepDefinitions
{
	public ResponseStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	[Then(@"the status code should be (.*)")]
	public void ThenTheStatusCodeShouldBe(int expectedStatusCode)
	{
		var response = _scenarioContext.GetResponse();
		response.StatusCode.Should().Be(expectedStatusCode);
	}

	[Then(@"response body is error")]
	public void ThenResponseBodyIsError()
	{
		var error = _scenarioContext.GetResponseBody<Error>();
		error.Should().NotBeNull();
	}

	private readonly ScenarioContext _scenarioContext;
}