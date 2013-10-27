using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IContainerAdapter
    {
        object GetInstance(Type type);
        IEnumerable<object> GetInstances(Type type);
    }
}
