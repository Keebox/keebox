using System;

using FluentAssertions;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class AccountManagementStepDefinitions
{
	public AccountManagementStepDefinitions(ScenarioContext scenarioContext, ApiRequestSender requestSender)
	{
		_scenarioContext = scenarioContext;
		_requestSender = requestSender;
	}

	[Then(@"accounts should be returned")]
	public void ThenAccountsShouldBeReturned()
	{
		var accounts = _scenarioContext.GetResponseBody<Account[]>();
		accounts.Should().NotBeNull();
	}

	[Then(@"account should be returned")]
	public void ThenAccountShouldBeReturned()
	{
		var account = _scenarioContext.GetResponseBody<Account>();

		account.Should().NotBeNull();
		account.Id.Should().Be(_scenarioContext.Get<string>(AccountIdKey));
		account.Name.Should().NotBeNull();
	}

	[Given(@"id of existing account")]
	public void GivenIdOfExistingAccount()
	{
		_requestSender.BecomeAdmin();
		var accountsRaw = _requestSender.GetAccounts();
		var accounts = JsonConvert.DeserializeObject<Account[]>(accountsRaw);
		_scenarioContext.Add(AccountIdKey, accounts[new Random().Next(accounts.Length - 1)].Id);
	}

	[Given(@"type and name is in the body")]
	public void GivenTypeAndNameIsInTheBody()
	{
		var request = _scenarioContext.GetRequest();
		var generateToken = _scenarioContext.Get<bool>(GenerateTokenKey);

		object payload;

		if (generateToken)
		{
			payload = new { Type = 1 /* Token Account */, Name = Guid.NewGuid().ToString(), GenerateToken = true };
		}
		else
		{
			var pregeneratedToken = _scenarioContext.Get<string>(PregeneratedTokenKey);

			payload = new
			{
				Type = 1 /* Token Account */, Name = Guid.NewGuid().ToString(), GenerateToken = false, Token = pregeneratedToken
			};
		}

		request.AddJsonBody(payload);
	}

	[Given(@"with api generated token")]
	public void GivenWithApiGeneratedToken()
	{
		_scenarioContext.Add(GenerateTokenKey, true);
	}

	[Given(@"with pregenerated api token")]
	public void GivenWithPregeneratedToken()
	{
		_scenarioContext.Add(GenerateTokenKey, false);
		_scenarioContext.Add(PregeneratedTokenKey, Guid.NewGuid().ToString());
	}

	[Then(@"private token should be returned")]
	public void ThenPrivateTokenShouldBeReturned()
	{
		_scenarioContext.GetResponseBody<string>().Should().NotBeNull();
	}

	[Then(@"private token should be like pregenerated")]
	public void ThenPrivateTokenShouldBeLikePregenerated()
	{
		_scenarioContext.GetResponseBody<string>().Should().Be(_scenarioContext.Get<string>(PregeneratedTokenKey));
	}

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;

	private const string AccountIdKey = "AccountId";
	private const string GenerateTokenKey = "GenerateToken";
	private const string PregeneratedTokenKey = "PregeneratedToken";
}