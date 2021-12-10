using Keebox.Common.Exceptions;
using Keebox.SecretsService.Managing;

using Microsoft.AspNetCore.Mvc.Filters;


namespace Keebox.SecretsService.Middlewares
{
	public class PrivilegedAccessAuthorizationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var user = (UserPrincipal)context.HttpContext.User;

			if (user.IsRootUser) return;
			if (user.HasSystemRole()) return;

			throw new RestrictedAccessException("You don't have permission to perform this action.");
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}