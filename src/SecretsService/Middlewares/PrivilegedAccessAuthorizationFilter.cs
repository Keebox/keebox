using Keebox.Common.Exceptions;
using Keebox.Common.Types;

using Microsoft.AspNetCore.Mvc.Filters;


namespace Keebox.SecretsService.Middlewares
{
	public class PrivilegedAccessAuthorizationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var user = context.HttpContext.User;

			if (user.IsInRole(FormattedSystemRole.Admin)) return;

			throw new RestrictedAccessException("You don't have permission to perform this action.");
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}