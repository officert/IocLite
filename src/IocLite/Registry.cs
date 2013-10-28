using System.Collections.Generic;
using System.Collections.ObjectModel;
using IocLite.Interfaces;
using IocLite.ObjectFactories;

namespace IocLite
{
    public abstract class Registry : IRegistry
    {
        private readonly ICollection<IBinding> _bindings;

        public ICollection<IBinding> Bindings
        {
            get
            {
                return _bindings;
            }
        }

        protected Registry()
        {
            _bindings = new Collection<IBinding>();
        }

        public virtual void Load()
        {
            //throw new NotImplementedException("You must provide your own implementation of Registry.Load()");
        }

        public DependencyMap<TAbstractType> For<TAbstractType>()
        {
            return new DependencyMap<TAbstractType>(this);
        }

        public void RegisterBinding(IBinding binding)
        {
            Ensure.ArgumentIsNotNull(binding, "binding");

            _bindings.Add(binding);
        }
    }

    public interface IRegistry
    {
        ICollection<IBinding> Bindings { get; }
        void Load();
        DependencyMap<TAbstractType> For<TAbstractType>();
        void RegisterBinding(IBinding binding);
    }
}
