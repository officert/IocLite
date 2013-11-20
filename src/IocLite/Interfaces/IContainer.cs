using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IContainer
    {
        object Resolve(Type type, string name = null);

        TService Resolve<TService>(string name = null);

        IEnumerable<object> ResolveAll(Type service);

        object TryResolve(Type service);

        //void Release(Type type);

        void Register(IList<IRegistry> registries);
    }
}
