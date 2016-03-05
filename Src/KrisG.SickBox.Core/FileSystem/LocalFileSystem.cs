using System.Collections.Generic;
using System.IO;
using System.Linq;
using KrisG.SickBox.Core.Interfaces.Data.FileSystem;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.FileSystem
{
    [ServiceImplementation("Local")]
    public class LocalFileSystem : IFileSystem
    {
        public ConnectionType Type
        {
            get { return ConnectionType.Local; }
        }

        public Stream OpenReadStream(string path)
        {
            return File.OpenRead(path);
        }

        public Stream OpenWriteStream(string path)
        {
            return File.OpenWrite(path);
        }

        public bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<FileEntry> ListFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).Select(x => new FileEntry(Path.GetFileName(x), x));
        }

        public FileSize GetFileSize(string path)
        {
            return new FileSize(new FileInfo(path).Length);
        }

        public string PathCombine(params string[] parts)
        {
            return Path.Combine(parts);
        }
    }
}