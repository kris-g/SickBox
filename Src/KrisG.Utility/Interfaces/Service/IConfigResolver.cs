using System;
using System.Xml.Linq;

namespace KrisG.Utility.Interfaces.Service
{
    public interface IConfigResolver
    {
        TConfig Get<TConfig>(XElement element);
        object Get(Type configType, XElement element);
    }
}