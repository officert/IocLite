using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IContainer
    {
        object Resolve(Type type, string name = null);
        object Resolve<T>(string name = null);
        IEnumerable<object> ResolveAll(Type type);

        object TryResolve(Type type);

        //void Release(Type type);

        void Register(IList<IRegistry> registries);
    }
}
