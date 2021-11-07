using System;
using System.Diagnostics;

using Keebox.Common.Types;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route("system")]
	public class SystemController : ControllerBase
	{
		public SystemController(Configuration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet("status")]
		public SystemInfo GetSystemInfo()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			var version = fileVersionInfo.ProductVersion ?? throw new InvalidOperationException();
			var uptime = (int)DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime).TotalMilliseconds;

			return new SystemInfo(version, _configuration.Engine.ToString()!, uptime);
		}

		[HttpGet("config")]
		public Config GetConfig()
		{
			return new Config
			{
				Engine = _configuration.Engine.ToString()!,
				DefaultFormat = _configuration.DefaultFormat.ToString()!,
				EnableWebUi = _configuration.EnableWebUi!.Value
			};
		}

		[HttpGet("start")]
		[ProducesResponseType(200)]
		public void Start()
		{
			// TODO: add start logic
		}

		[HttpGet("stop")]
		[ProducesResponseType(200)]
		public void Stop()
		{
			// TODO: add stop logic
		}

		private readonly Configuration _configuration;
	}
}