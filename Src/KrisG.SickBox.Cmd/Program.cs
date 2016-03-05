using System;
using KrisG.SickBox.Core;

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

            var downloader = DefaultDownloader.Create();
            downloader.Execute();
        }
    }
}
