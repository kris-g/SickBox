using System.Collections.Generic;
using System.IO;
using KrisG.SickBox.Core.Interfaces.Data.FileSystem;
using KrisG.SickBox.Core.Interfaces.Enums;

namespace KrisG.SickBox.Core.Interfaces.FileSystem
{
    public interface IFileSystem
    {
        ConnectionType Type { get; }

        Stream OpenReadStream(string path);
        Stream OpenWriteStream(string path);
        void CompleteOperation();
        bool DeleteFile(string path);
        IEnumerable<FileEntry> ListFiles(string directoryPath);
        IEnumerable<FileEntry> ListDirectories(string directoryPath);
        FileSize GetFileSize(string path);
        string PathCombine(params string[] parts);
    }
}