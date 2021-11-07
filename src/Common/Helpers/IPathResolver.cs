using Keebox.Common.Types;


namespace Keebox.Common.Helpers
{
	public interface IPathResolver
	{
		PathType Resolve(string path);

		public (string name, string path) DeconstructPath(string fullPath);

		public bool ValidatePath(string path);
	}
}