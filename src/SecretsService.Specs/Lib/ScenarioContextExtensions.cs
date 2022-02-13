using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Lib;

public static class ScenarioContextExtensions
{
	public static IRestResponse GetResponse(this ScenarioContext context)
	{
		return context.Get<IRestResponse>(ContextKeys.Response);
	}

	public static void SetResponse(this ScenarioContext context, IRestResponse response)
	{
		context.Set(response, ContextKeys.Response);
	}

	public static RestRequest GetRequest(this ScenarioContext context)
	{
		RestRequest request;
		if (!context.ContainsKey(ContextKeys.Request))
		{
			request = new RestRequest();
			SetRequest(context, request);
		}
		else
		{
			request = context.Get<RestRequest>(ContextKeys.Request);
		}

		return request;
	}

	public static void SetRequest(this ScenarioContext context, RestRequest request)
	{
		context.Set(request, ContextKeys.Request);
	}

	public static T GetResponseBody<T>(this ScenarioContext context)
	{
		var content = GetResponse(context).Content;

		return JsonConvert.DeserializeObject<T>(content);
	}
}