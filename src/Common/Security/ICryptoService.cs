namespace Keebox.Common.Security
{
	public interface ICryptoService
	{
		string GetHash(string input);
	}
}