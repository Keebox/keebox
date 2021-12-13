using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;

using Keebox.Common.Types;
using Keebox.SecretsService.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;


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
		[OpenApiOperation("Get system info", "Gets info about running application")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(SystemInfo))]
		public ActionResult<SystemInfo> GetSystemInfo()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			var version = fileVersionInfo.ProductVersion ?? throw new InvalidOperationException();
			var uptime = (int)DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime).TotalMilliseconds;

			return Ok(new SystemInfo(version, _configuration.Engine.ToString()!, uptime));
		}

		[HttpGet(RouteMap.System.Config)]
		[OpenApiOperation("Get config", "Gets current application config")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(Config))]
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
		[OpenApiOperation("Start", "Start accepting secrets request")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public void Start()
		{
			// TODO: add start logic
		}

		[HttpGet(RouteMap.System.Stop)]
		[OpenApiOperation("Stop", "Stop accepting secrets request")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public void Stop()
		{
			// TODO: add stop logic
		}

		private readonly Configuration _configuration;
	}
}