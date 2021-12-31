using System;

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

		var adminJwtToken = Login(token, out _);
		_internalClient.Authenticator = new JwtAuthenticator(JsonConvert.DeserializeObject<string>(adminJwtToken));
	}

	public ApiRequestSender ChangeToAccount(string token)
	{
		var jwtToken = Login(token, out _);
		_internalClient.Authenticator = new JwtAuthenticator(JsonConvert.DeserializeObject<string>(jwtToken));

		return this;
	}

	public ApiRequestSender EraseToken()
	{
		_internalClient.Authenticator = null;

		return this;
	}

	public ApiRequestSender BecomeAdmin()
	{
		var adminJwtToken = Login(ConfigurationHelper.AdminToken, out _);

		_internalClient.Authenticator = new JwtAuthenticator(JsonConvert.DeserializeObject<string>(adminJwtToken));

		return this;
	}

	public string CreateAccount(Account account)
	{
		var request = new RestRequest
		{
			Method = Method.POST,
			Resource = Endpoints.AccountEndpoint,
			Parameters = { new Parameter(ContentType, JsonConvert.SerializeObject(account), ParameterType.RequestBody) }
		};

		var response = _internalClient.Execute(request);

		return response.Content;
	}

	public string Login(string privateToken, out IRestResponse response)
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

		response = _internalClient.Execute(request);

		return response.Content;
	}

	public string CreateRole(string name, out IRestResponse response)
	{
		var request = new RestRequest
		{
			Method = Method.POST,
			Resource = Endpoints.RoleEndpoint,
			Parameters = { new Parameter(ContentType, JsonConvert.SerializeObject(new { Name = name }), ParameterType.RequestBody) }
		};

		response = _internalClient.Execute(request);

		return response.Content;
	}

	public IRestResponse SendRequest(RestRequest request)
	{
		return _internalClient.Execute(request);
	}

	private const string ContentType = "application/json";

	private readonly IRestClient _internalClient;
}