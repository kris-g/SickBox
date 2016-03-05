using System;
using System.IO;
using System.Xml.Linq;
using KrisG.Utility.Interfaces.Configuration;

namespace KrisG.Utility.Service
{
    public class CommandLineConfigurationFileReader : IConfigurationReader
    {
        public XDocument Read()
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                throw new InvalidOperationException("No command args found for loading configuration file");
            }

            var configPath = args[1];
            if (!File.Exists(configPath))
            {
                throw new InvalidOperationException(string.Format("Command arg configuration path {0} does not exist", configPath));
            }

            return XDocument.Load(File.OpenText(configPath));
        }
    }
}