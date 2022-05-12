namespace Services.Extensions;

public static class StringExtension
{
	private const string PathSeparator = "/";

	public static (string path, string name) DeconstructPath(this string fullPath)
	{
		var splitted = fullPath.Split(PathSeparator);
		var name = splitted.Last();
		var path = string.Join(PathSeparator, splitted.Take(splitted.Length - 1));

		return (path, name);
	}
}