using System;
using System.Linq;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Serializable]
public record LoginData
{
	public int StatusCode;
	public string AccountAccessToken;
	public string AccountPrivateToken;
	public RestResponseCookie[] Cookies;
}

[Binding]
public class LoginPreparationStepDefinitions
{
	public LoginPreparationStepDefinitions(LoginData loginData)
	{
		_loginData = loginData;
	}

	[Given(@"a temporary account")]
	public void GivenATemporaryAccount()
	{
		var requestSender = new ApiClient().WithToken(ConfigurationHelper.AdminToken);

		_loginData.AccountPrivateToken = requestSender.CreateAccount(new Account
		{
			Type = 1,
			Name = $"auto-{Guid.NewGuid().ToString().ToLower()}",
			GenerateToken = true
		});
	}

	private readonly LoginData _loginData;
}

[Binding]
public class LoginStepDefinitions
{
	public LoginStepDefinitions(LoginData loginData)
	{
		_loginData = loginData;
		_requestSender = new ApiClient().WithoutToken();
	}

	[Given(@"the account token")]
	public void GivenTheAccountToken()
	{
		/* Account Token already set in constructor */
	}

	[When(@"the login request has been sent")]
	public void WhenTheLoginRequestHasBeenSent()
	{
		var jwtToken = _requestSender.Login(_loginData.AccountPrivateToken, out var statusCode, out var cookies);

		_loginData.AccountAccessToken = jwtToken;
		_loginData.StatusCode = statusCode;
		_loginData.Cookies = cookies;
	}

	[Then(@"the status code should be (.*)")]
	public void ThenTheStatusCodeShouldBe(int expectedStatusCode)
	{
		_loginData.StatusCode.Should().Be(expectedStatusCode);
	}

	[Then(@"access token should be returned")]
	public void ThenAccessTokenShouldBeReturned()
	{
		_loginData.AccountAccessToken.Should().NotBeNullOrEmpty();
	}

	[Then(@"access token should be in cookie")]
	public void ThenAccessTokenShouldBeInCookie()
	{
		_loginData.Cookies.Single(cookie => cookie.Name.Equals("access-token")).Value.Should().NotBeNullOrEmpty();
	}

	private readonly LoginData _loginData;
	private readonly ApiRequestSender _requestSender;
}