using Keebox.Common.Types;


namespace Keebox.Common.Managers
{
	public interface IConfigurationManager
	{
		Configuration? Get(string path);

		Configuration Merge(Configuration old, Configuration @new);

		Configuration GetDefaultConfiguration();

		void Save(string path, Configuration configuration);
	}
}