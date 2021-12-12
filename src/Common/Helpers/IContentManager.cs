namespace Keebox.Common.Helpers
{
	public interface IContentManager
	{
		public string? Get(string path);

		public byte[]? GetRaw(string path);

		public void Save(string path, string content);

		public void Save(string path, byte[] contentRaw);
	}
}