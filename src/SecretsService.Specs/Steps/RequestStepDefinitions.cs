using System.Linq;
using System.Text.RegularExpressions;

using Keebox.SecretsService.Specs.Lib;

using RestSharp;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Steps;

[Binding]
public class RequestStepDefinitions
{
	public RequestStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	[Given(@"method is '(.*)'")]
	public void GivenMethodIs(Method method)
	{
		var request = _scenarioContext.GetRequest();
		request.Method = method;
		if (method == Method.GET)
		{
			request.Body = null;
			var header = request.Parameters.Find(p => p.Type == ParameterType.RequestBody);
			request.Parameters.Remove(header);
		}
	}

	[Given(@"endpoint is '(.*)'")]
	public void GivenEndpointIs(string endpoint)
	{
		if (KeyRegex.IsMatch(endpoint))
		{
			var groups = KeyRegex.Match(endpoint).Groups.Values.Skip(1).ToArray();
			foreach (var g in groups)
			{
				var value = _scenarioContext.Get<string>(g.Value[1..^1]);
				endpoint = endpoint.Replace(g.Value, value);
			}
		}

		_scenarioContext.GetRequest().Resource = endpoint;
	}

	private readonly Regex KeyRegex = new Regex("(\\{.+\\})");

	private readonly ScenarioContext _scenarioContext;
}