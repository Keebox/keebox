using System;
using System.IO;
using System.Text;


namespace Keebox.Common.Helpers
{
	public class ContentManager : IContentManager
	{
		public string? Get(string path)
		{
			return !File.Exists(path) ? null : File.ReadAllText(path);
		}

		public byte[]? GetRaw(string path)
		{
			return !File.Exists(path) ? null : File.ReadAllBytes(path);
		}

		public void Save(string path, string content)
		{
			CreateDirectoryIfNotExists(path);

			var file = File.Create(path);

			file.Write(Encoding.UTF8.GetBytes(content));
			file.Close();
		}

		public void Save(string path, byte[] contentRaw)
		{
			CreateDirectoryIfNotExists(path);

			var file = File.Create(path);

			file.Write(contentRaw);
			file.Close();
		}

		private static void CreateDirectoryIfNotExists(string path)
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
			}
		}
	}
}