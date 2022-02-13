using System.Collections.Generic;

using Keebox.Common.DataAccess.Entities;
using Keebox.Common.Types;


namespace Keebox.Common.Managers
{
	public interface ISecretManager
	{
		(PathType,IEnumerable<Secret>) GetSecrets(string path, string[] secrets);

		void AddSecrets(string path, Dictionary<string, string> input, Dictionary<string, string> files, string[] secrets);

		void DeleteSecrets(string path, string[] secrets);
	}
}