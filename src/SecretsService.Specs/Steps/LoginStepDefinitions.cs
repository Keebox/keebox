using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class LoginStepDefinitions
{
	public LoginStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	[Given(@"the valid account token")]
	public void GivenTheValidAccountToken()
	{
		var temporaryAccountToken = _scenarioContext.Get<string>("CreatedAccountToken");
		_scenarioContext.Add("AccountToken", temporaryAccountToken);
	}

	[Given(@"the wrong account token")]
	public void GivenTheWrongAccountToken()
	{
		_scenarioContext.Add("AccountToken", _fixture.Create<string>());
	}

	[Then(@"access token should be returned")]
	public void ThenAccessTokenShouldBeReturned()
	{
		_scenarioContext.GetBody<string>().Should().NotBeNullOrEmpty();
	}

	[Then(@"access token should be in cookie")]
	public void ThenAccessTokenShouldBeInCookie()
	{
		_scenarioContext.GetResponse().Cookies.Single(cookie => cookie.Name.Equals("access-token")).Value.Should().NotBeNullOrEmpty();
	}

	private Fixture _fixture = new();

	private readonly ScenarioContext _scenarioContext;
}