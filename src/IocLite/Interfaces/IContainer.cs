using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IContainer
    {
        DependencyMap<TAbstractType> For<TAbstractType>();

        object Resolve(Type type);
        object Resolve<T>();
        IEnumerable<object> ResolveAll(Type type);

        object TryResolve(Type type);

        void Release(Type type);

        void RegisterBinding(IBinding binding);
    }
}
