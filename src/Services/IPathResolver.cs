namespace Services;

public interface IPathResolver
{
	PathType Resolve(string path);
}