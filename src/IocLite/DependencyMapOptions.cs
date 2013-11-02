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

        /// <summary>
        /// Default scope. Specifies that the object should not be cached. The container will always return a new instance.
        /// </summary>
        public void InTransientScope()
        {
            _binding.ObjectScope = ObjectScope.Transient;
        }

        /// <summary>
        /// Specifies that the object should be cached as a singleton. The container will return the same instance for the duration of the application.
        /// </summary>
        public void InSingletonScope()
        {
            _binding.ObjectScope = ObjectScope.Singleton;
        }

        /// <summary>
        /// Specifies that the object should be cached per http request. The container will return the same instance for the duration of the http request.
        /// </summary>
        public void InHttpRequestScope()
        {
            _binding.ObjectScope = ObjectScope.HttpRequest;
        }

        /// <summary>
        /// Specifies that the object should be cached per thread. The container will return the same instance for the duration of the thread.
        /// </summary>
        public void InThreadScope()
        {
            _binding.ObjectScope = ObjectScope.ThreadScope;
        }
    }

    public interface IDependencyMapOptions
    {
        IDependencyMapOptions Named(string name);
        /// <summary>
        /// Default scope. Specifies that the object should not be cached. The container will always return a new instance.
        /// </summary>
        void InTransientScope();
        /// <summary>
        /// Specifies that the object should be cached as a singleton. The container will return the same instance for the duration of the application.
        /// </summary>
        void InSingletonScope();
        /// <summary>
        /// Specifies that the object should be cached per http request. The container will return the same instance for the duration of the http request.
        /// </summary>
        void InHttpRequestScope();
        /// <summary>
        /// Specifies that the object should be cached per thread. The container will return the same instance for the duration of the thread.
        /// </summary>
        void InThreadScope();
    }
}
