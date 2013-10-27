using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IContainer
    {
        object Resolve(Type type);
        object Resolve<T>();
        IEnumerable<object> ResolveAll(Type type);

        object TryResolve(Type type);

        void Release(Type type);

        void Register(IList<IRegistry> registries);
    }
}
