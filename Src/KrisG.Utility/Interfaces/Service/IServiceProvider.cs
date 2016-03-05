using System.Collections.Generic;

namespace KrisG.Utility.Interfaces.Service
{
    public interface IServiceProvider
    {
        TService Get<TService>(bool optionalService = false);
        IEnumerable<TService> GetAll<TService>(bool optionalService = false);
    }
}