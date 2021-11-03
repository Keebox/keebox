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

        public void Save(string path, string content)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
            }

            var file = File.Create(path);
            file.Write(Encoding.UTF8.GetBytes(content));
            file.Close();
        }
    }
}