using System;
namespace IocLite.Interfaces
{
    public interface IContainerAdapter
    {
        object TryResolve(Type type);
    }
}
