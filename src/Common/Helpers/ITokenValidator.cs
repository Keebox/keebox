namespace Keebox.Common.Helpers
{
	public interface ITokenValidator
	{
		bool Validate(string token);

		bool ValidateHash(string tokenHash);
	}
}