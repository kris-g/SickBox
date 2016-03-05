using System.Diagnostics;

namespace KrisG.SickBox.Core.Interfaces.Data.FileSystem
{
    [DebuggerDisplay("{Name}")]
    public class FileEntry
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public FileEntry(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}