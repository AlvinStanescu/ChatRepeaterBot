using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHistoryParser.Interfaces
{
    public interface IFileUtility
    {
        bool Exists(string path);

        void Delete(string path);

        void WriteAllText(string path, string contents);
    }
}
