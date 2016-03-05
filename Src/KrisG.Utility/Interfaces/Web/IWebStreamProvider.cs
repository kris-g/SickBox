using System;
using System.IO;

namespace KrisG.Utility.Interfaces.Web
{
    public interface IWebStreamProvider
    {
        TimeSpan Timeout { get; set; }
        Stream GetStream(string url);
    }
}