using System.Security.Claims;


namespace Keebox.Common.Helpers
{
	public interface ITokenValidator
	{
		bool Validate(string token);

		bool ValidateHash(string tokenHash);

		bool ValidateJwtToken(string jwtToken, out ClaimsPrincipal? identity);
	}
}