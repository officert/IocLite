using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class SingletonObjectFactory : IObjectFactory
    {
        private object _objInstance;

        public SingletonObjectFactory()
        {
        }

        public SingletonObjectFactory(object instance)
        {
            _objInstance = instance;
        }

        public object GetObject(IBinding binding, Container container)
        {
            return _objInstance ?? (_objInstance = container.CreateObjectGraph(binding.ServiceType));
        }
    }
}
