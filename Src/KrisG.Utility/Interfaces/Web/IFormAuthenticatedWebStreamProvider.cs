using System;
using System.IO;

namespace KrisG.Utility.Interfaces.Web
{
    public interface IFormAuthenticatedWebStreamProvider : IDisposable
    {
        string Username { get; set; }

        string Password { get; set; }

        Stream GetStream(string url);
        void DownloadFile(string url, string localFilePath);
    }
}