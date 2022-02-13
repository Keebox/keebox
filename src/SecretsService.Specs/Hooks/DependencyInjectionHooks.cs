using BoDi;

using Keebox.SecretsService.Specs.Lib;

using TechTalk.SpecFlow;


namespace Keebox.SecretsService.Specs.Hooks;

[Binding]
public class DependencyInjectionHooks
{
	public DependencyInjectionHooks(IObjectContainer container)
	{
		_container = container;
	}

	[BeforeScenario]
	public void RegisterApiClient()
	{
		_container.RegisterInstanceAs(new ApiClient().WithoutToken());
	}

	private readonly IObjectContainer _container;
}