using ChatHistoryParser.Interfaces;
using System.IO;

namespace ChatHistoryParser.Utility
{
    public class FileUtility : IFileUtility
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}
