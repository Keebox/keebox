using System.Net;

using Keebox.Common.Exceptions;
using Keebox.SecretsService.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;


namespace Keebox.SecretsService.RequestFiltering
{
	public class ExceptionsFilter : IExceptionFilter
	{
		private readonly ILogger<ExceptionsFilter> _logger;

		public ExceptionsFilter(ILogger<ExceptionsFilter> logger) =>
			_logger = logger;

		public void OnException(ExceptionContext context)
		{
			var status = HttpStatusCode.InternalServerError;
			var message = "Internal sever error.";

			switch (context.Exception)
			{
				case NotFoundException:
					status = HttpStatusCode.NotFound;
					message = context.Exception.Message;

					break;

				case UnauthorizedException:
					status = HttpStatusCode.Unauthorized;
					message = context.Exception.Message;

					break;

				case RestrictedAccessException:
					status = HttpStatusCode.Forbidden;
					message = context.Exception.Message;

					break;

				default:
					_logger.LogError(context.Exception, message);

					break;
			}

			context.ExceptionHandled = true;
			context.HttpContext.Response.StatusCode = (int)status;
			context.HttpContext.Response.ContentType = "application/json";

			context.Result = new ObjectResult(new Error((int)status, message));
		}
	}
}