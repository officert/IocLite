using IocLite.Interfaces;

namespace IocLite
{
    public class DependencyOptions : IDependencyOptions
    {
        private readonly IBinding _binding;

        public DependencyOptions(IBinding binding)
        {
            _binding = binding;
        }

        public DependencyOptions Named(string name)
        {
            _binding.Name = name;
            return new DependencyOptions(_binding);
        }

        public void InSingletonScope()
        {
            _binding.ObjectScope = ObjectScope.Singleton;
        }

        public void InTransientScope()
        {
            _binding.ObjectScope = ObjectScope.Transient;
        }
    }

    public interface IDependencyOptions
    {
        DependencyOptions Named(string name);
        void InSingletonScope();
        void InTransientScope();
    }
}
