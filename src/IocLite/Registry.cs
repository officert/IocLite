using System.Collections.Generic;
using System.Collections.ObjectModel;
using IocLite.Interfaces;

namespace IocLite
{
    public abstract class Registry : IRegistry
    {
        private readonly ICollection<IBinding> _bindings;

        public IEnumerable<IBinding> Bindings
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

        public DependencyMap<TServiceType> For<TServiceType>()
        {
            return new DependencyMap<TServiceType>(this);
        }

        public void RegisterBinding(IBinding binding)
        {
            Ensure.ArgumentIsNotNull(binding, "binding");

            _bindings.Add(binding);
        }
    }
}
