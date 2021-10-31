namespace Keebox.Common.Helpers
{
	public interface IContentManager
	{
		public string? Get(string path);

		public void Save(string path, string content);
	}
}