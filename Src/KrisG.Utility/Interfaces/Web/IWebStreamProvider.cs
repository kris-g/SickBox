using System.IO;

namespace KrisG.Utility.Interfaces.Web
{
    public interface IWebStreamProvider
    {
        Stream GetStream(string url);
    }
}