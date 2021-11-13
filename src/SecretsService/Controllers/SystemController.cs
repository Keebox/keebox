using System;
using System.Diagnostics;

using Keebox.Common.Types;
using Keebox.SecretsService.Models;
using Keebox.SecretsService.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Keebox.SecretsService.Controllers
{
	[ApiController]
	[Route(RouteMap.System.Base)]
	public class SystemController : ControllerBase
	{
		public SystemController(Configuration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet(RouteMap.System.Status)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<SystemInfo> GetSystemInfo()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			var version = fileVersionInfo.ProductVersion ?? throw new InvalidOperationException();
			var uptime = (int)DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime).TotalMilliseconds;

			return Ok(new SystemInfo(version, _configuration.Engine.ToString()!, uptime));
		}

		[HttpGet(RouteMap.System.Config)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<Config> GetConfig()
		{
			return Ok(new Config
			{
				Engine = _configuration.Engine.ToString()!,
				DefaultFormat = _configuration.DefaultFormat.ToString()!,
				EnableWebUi = _configuration.EnableWebUi!.Value
			});
		}

		[HttpGet(RouteMap.System.Start)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public void Start()
		{
			// TODO: add start logic
		}

		[HttpGet(RouteMap.System.Stop)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public void Stop()
		{
			// TODO: add stop logic
		}

		private readonly Configuration _configuration;
	}
}