using System;
using System.Collections.Generic;

namespace IocLite
{
    public interface IContainerAdapter
    {
        object GetInstance(Type type);
        IEnumerable<object> GetInstances(Type type);
    }
}
