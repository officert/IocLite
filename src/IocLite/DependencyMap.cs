using System;
using IocLite.Interfaces;

namespace IocLite
{
    public class DependencyMap<TAbstractType>
    {
        private readonly IRegistry _registry;
        private readonly Binding _binding;

        public DependencyMap(IRegistry registry)
        {
            _registry = registry;

            _binding = new Binding
            {
                PluginType = typeof(TAbstractType),
                ServiceType = ConcreteType
            };
        }

        public Type ConcreteType { get; private set; }

        public Type AbstractType
        {
            get
            {
                return _binding.PluginType;
            }
        }

        public string Name { get; set; }

        public object Instance { get; set; }

        public DependencyOptions Use<TConcreteType>() where TConcreteType : TAbstractType
        {
            _binding.ServiceType = typeof(TConcreteType);

            _registry.RegisterBinding(_binding);

            return new DependencyOptions(_binding);
        }

        public DependencyOptions Use<TConcreteType>(TConcreteType type) where TConcreteType : TAbstractType
        {
            _binding.ServiceType = typeof(TConcreteType);
            _binding.Instance = type;
            _binding.ObjectScope = ObjectScope.Singleton;   //if you provide an instance, the registration will be a singleton instance

            _registry.RegisterBinding(_binding);

            return new DependencyOptions(_binding);
        }
    }
}
