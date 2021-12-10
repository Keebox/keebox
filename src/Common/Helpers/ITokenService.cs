using System;

using Keebox.Common.DataAccess.Entities;


namespace Keebox.Common.Helpers
{
	public interface ITokenService
	{
		string GenerateStatelessToken();

		string GenerateJwtToken(Guid userId, Role[] roles);
	}
}