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

        public void InDefaultScope()
        {
            _binding.ObjectScope = ObjectScope.Default;
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

    public interface IDependencyOptions
    {
        DependencyOptions Named(string name);
        void InDefaultScope();
        void InSingletonScope();
        void InHttpRequestScope();
        void InThreadScope();
    }
}
