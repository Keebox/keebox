namespace Keebox.Common.Helpers
{
	public interface IKeyProvider
	{
		byte[] GetTokenSigningKey();
	}
}