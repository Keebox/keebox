using System.Linq;

using AutoFixture;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class LoginStepDefinitions
{
	public LoginStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	[Given(@"private token in the body")]
	public void GivenPrivateTokenInTheBody()
	{
		var temporaryAccountToken = _scenarioContext.Get<string>("CreatedAccountToken");
		var request = _scenarioContext.GetRequest();
		request.AddJsonBody(new { Token = temporaryAccountToken });
	}

	[Given(@"the wrong account token")]
	public void GivenTheWrongAccountToken()
	{
		_scenarioContext.Add("AccountToken", _fixture.Create<string>());
	}

	[Then(@"access token should be returned")]
	public void ThenAccessTokenShouldBeReturned()
	{
		_scenarioContext.GetResponse().Content.Should().NotBeNull();
	}

	[Then(@"access token should be in cookie")]
	public void ThenAccessTokenShouldBeInCookie()
	{
		_scenarioContext.GetResponse().Cookies.Single(cookie => cookie.Name.Equals("access-token")).Value.Should().NotBeNullOrEmpty();
	}

	private readonly Fixture _fixture = new();

	private readonly ScenarioContext _scenarioContext;
}