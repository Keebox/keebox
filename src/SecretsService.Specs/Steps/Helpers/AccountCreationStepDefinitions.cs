using System;

using Keebox.SecretsService.Specs.Lib;
using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps.Helpers;

[Binding]
public class AccountCreationStepDefinitions
{
	public AccountCreationStepDefinitions(ScenarioContext scenarioContext, ApiRequestSender apiRequestSender)
	{
		_scenarioContext = scenarioContext;
		_requestSender = apiRequestSender;
	}

	[Given(@"account token set to request body")]
	public void GivenAccountTokenSetToRequestBody()
	{
		var request = _scenarioContext.GetRequest();
		var privateToken = _scenarioContext.Get<string>("AccountToken");
		request.AddJsonBody(new { Token = privateToken });
	}

	[Given(@"an ordinary account and its token")]
	public void GivenAnOrdinaryAccountAndItsToken()
	{
		_requestSender.BecomeAdmin();

		var temporaryAccountToken = _requestSender.CreateAccount(new Account
		{
			Type = 1,
			Name = $"auto-{Guid.NewGuid().ToString().ToLower()}",
			GenerateToken = true
		});

		temporaryAccountToken = JsonConvert.DeserializeObject<string>(temporaryAccountToken);

		_requestSender.ChangeToAccount(temporaryAccountToken);
		_scenarioContext.Add("CreatedAccountToken", temporaryAccountToken);
	}

	[Given(@"an admin account and its token")]
	public void GivenAnAdminAccountAndItsToken()
	{
		_requestSender.BecomeAdmin();
	}

	private readonly ScenarioContext _scenarioContext;
	private readonly ApiRequestSender _requestSender;
}