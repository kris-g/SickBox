using System;
using System.IO;
using System.Reflection;
using KrisG.SickBox.Core;
using log4net;
using log4net.Config;

namespace KrisG.SickBox.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: SickBox.exe [ConfigPath]");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var downloader = DefaultDownloader.Create();
            downloader.Execute();
        }
    }
}