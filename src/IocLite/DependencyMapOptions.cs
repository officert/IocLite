using IocLite.Interfaces;

namespace IocLite
{
    public class DependencyMapMapOptions : IDependencyMapOptions
    {
        private readonly IBinding _binding;

        public DependencyMapMapOptions(IBinding binding)
        {
            _binding = binding;
        }

        public IDependencyMapOptions Named(string name)
        {
            _binding.Name = name;
            return new DependencyMapMapOptions(_binding);
        }

        public void InTransientScope()
        {
            _binding.ObjectScope = ObjectScope.Transient;
        }

        public void InSingletonScope()
        {
            _binding.ObjectScope = ObjectScope.Singleton;
        }

        public void InHttpRequestScope()
        {
            _binding.ObjectScope = ObjectScope.HttpRequest;
        }

        public void InThreadScope()
        {
            _binding.ObjectScope = ObjectScope.ThreadScope;
        }
    }

    public interface IDependencyMapOptions
    {
        IDependencyMapOptions Named(string name);
        void InTransientScope();
        void InSingletonScope();
        void InHttpRequestScope();
        void InThreadScope();
    }
}
