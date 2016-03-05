using System.Xml.Linq;

namespace KrisG.Utility.Interfaces.Configuration
{
    public interface IConfigurationReader
    {
        XDocument Read();
    }
}