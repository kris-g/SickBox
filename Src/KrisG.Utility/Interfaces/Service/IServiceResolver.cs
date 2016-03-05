using System.Xml.Linq;

namespace KrisG.Utility.Interfaces.Service
{
    public interface IServiceResolver
    {
        TService Get<TService>(string implementationName, XElement element);
    }
}