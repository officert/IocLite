using IocLite.Interfaces;

namespace IocLite
{
    public class DependencyMap<TServiceType>
    {
        private readonly IRegistry _registry;
        private readonly Binding _binding;

        public DependencyMap(IRegistry registry)
        {
            Ensure.ArgumentIsNotNull(registry, "registry");

            _registry = registry;

            _binding = new Binding
            {
                ServiceType = typeof(TServiceType)
            };
        }

        public string Name { get; set; }

        public object Instance { get; set; }

        public DependencyOptions Use<TPluginType>() where TPluginType : TServiceType
        {
            _binding.PluginType = typeof(TPluginType);

            _registry.RegisterBinding(_binding);

            return new DependencyOptions(_binding);
        }

        public DependencyOptions Use<TPluginType>(TPluginType type) where TPluginType : TServiceType
        {
            _binding.Instance = type;
            _binding.ObjectScope = ObjectScope.Singleton;   //if you provide an instance, the registration will default to singleton scope

            _binding.PluginType = typeof(TPluginType);
            _registry.RegisterBinding(_binding);

            return new DependencyOptions(_binding);
        }
    }
}
