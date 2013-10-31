using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IRegistry
    {
        IEnumerable<IBinding> Bindings { get; }
        void Load();
        DependencyMap<TAbstractType> For<TAbstractType>();
        void RegisterBinding(IBinding binding);
    }
}
