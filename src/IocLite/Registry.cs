using System.Collections.Generic;
using System.Collections.ObjectModel;
using IocLite.Interfaces;
using IocLite.ObjectFactories;

namespace IocLite
{
    public abstract class Registry : IRegistry
    {
        private readonly ICollection<BindingRegistration> _bindingRegistrations;
 
        public ICollection<BindingRegistration> BindingRegistrations
        {
            get
            {
                return _bindingRegistrations;
            }
        }

        protected Registry()
        {
            _bindingRegistrations = new Collection<BindingRegistration>();

            Load();
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

            BindingRegistrations.Add(new BindingRegistration
            {
                Binding = binding
            });
        }
    }

    public interface IRegistry
    {
        ICollection<BindingRegistration> BindingRegistrations { get; }
        void Load();
        DependencyMap<TAbstractType> For<TAbstractType>();
        void RegisterBinding(IBinding binding);
    }
}
