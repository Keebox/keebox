using System;
using System.Linq;

using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;


namespace Keebox.SecretsService.Specs.Lib;

public class ApiClient
{
	public ApiRequestSender WithToken(string token)
	{
		return new ApiRequestSender(ConfigurationHelper.ApiBase, token);
	}

	public ApiRequestSender WithoutToken()
	{
		return new ApiRequestSender(ConfigurationHelper.ApiBase);
	}
}

public class ApiRequestSender
{
	public ApiRequestSender(string baseUrl)
	{
		_internalClient = new RestClient
		{
			BaseUrl = new Uri(baseUrl)
		};
	}

	public ApiRequestSender(string baseUrl, string token)
	{
		_internalClient = new RestClient
		{
			BaseUrl = new Uri(baseUrl)
		};

		var adminJwtToken = Login(token, out _, out _);

		_internalClient.Authenticator = new JwtAuthenticator(adminJwtToken);
	}

	public ApiRequestSender ChangeTokenTo(string token)
	{
		_internalClient.Authenticator = new JwtAuthenticator(token);
		return this;
	}

	public string CreateAccount(Account account)
	{
		var request = new RestRequest
		{
			Method = Method.POST,
			Resource = Endpoints.AccountCreationEndpoint,
			Parameters = { new Parameter(ContentType, JsonConvert.SerializeObject(account), ParameterType.RequestBody) }
		};

		var response = _internalClient.Execute<string>(request);

		if (!response.IsSuccessful) throw new InvalidOperationException($"Cannot send request. Status code: {response.StatusCode}");

		return response.Data;
	}

	public string Login(string privateToken, out int statusCode, out RestResponseCookie[] cookies)
	{
		var request = new RestRequest
		{
			Method = Method.POST,
			Resource = Endpoints.LoginEndpoint,
			Parameters =
			{
				new Parameter(ContentType, JsonConvert.SerializeObject(new { Token = privateToken }), ParameterType.RequestBody)
			}
		};

		var response = _internalClient.Execute<string>(request);

		statusCode = (int)response.StatusCode;
		cookies = response.Cookies.ToArray();

		return response.Data;
	}

	private const string ContentType = "application/json";

	private readonly IRestClient _internalClient;
}