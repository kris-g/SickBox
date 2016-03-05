using System.IO;

namespace KrisG.Utility.Interfaces
{
    public interface IStreamCopier
    {
        void Copy(Stream from, Stream to, string toPath, long totalSize);
    }
}