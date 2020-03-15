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
            var path = @"/config/configuration.xml";

            if (!File.Exists(path))
            {
                throw new InvalidOperationException($"Configuration file not found [Path: {path}]");
            }

            return XDocument.Load(File.OpenText(path));
        }
    }
}