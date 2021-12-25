using Keebox.SecretsService.Specs.Lib.Models;

using Newtonsoft.Json;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Lib;

public static class ScenarioContextExtensions
{
	public static T GetBody<T>(this ScenarioContext context)
	{
		return context.Get<T>(ContextKeys.Body);
	}

	public static void SetBody<T>(this ScenarioContext context, string body)
	{
		context.Set(JsonConvert.DeserializeObject<T>(body), ContextKeys.Body);
	}

	public static Error GetError(this ScenarioContext context)
	{
		return context.Get<Error>(ContextKeys.Error);
	}

	public static void SetError(this ScenarioContext context, string errorBody)
	{
		context.Set(JsonConvert.DeserializeObject<Error>(errorBody), ContextKeys.Error);
	}

	public static IRestResponse GetResponse(this ScenarioContext context)
	{
		return context.Get<IRestResponse>(ContextKeys.Response);
	}

	public static void SetResponse(this ScenarioContext context, IRestResponse response)
	{
		context.Set(response, ContextKeys.Response);
	}
}