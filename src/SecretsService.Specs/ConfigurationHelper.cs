using System;

using Microsoft.Extensions.Configuration;


namespace Keebox.SecretsService.Specs;

public static class ConfigurationHelper
{
	static ConfigurationHelper()
	{
		_configuration = new ConfigurationBuilder()
			.SetBasePath(Environment.CurrentDirectory)
			.AddJsonFile("appsettings.json", true, true)
			.Build();
	}

	public static string ApiBase
	{
		get => _configuration["ApiBaseUrl"];
	}

	public static string AdminToken
	{
		get => Environment.GetEnvironmentVariable("KEEBOX_AUTOTEST_ACCOUNT_TOKEN")!.Trim();
	}

	private static readonly IConfiguration _configuration;
}