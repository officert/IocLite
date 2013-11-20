using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class SingletonInstanceObjectFactory : IObjectFactory
    {
        private object _objInstance;

        public SingletonInstanceObjectFactory()
        {
        }

        public SingletonInstanceObjectFactory(object instance)
        {
            _objInstance = instance;
        }

        public object GetObject(IBinding binding, Container container)
        {
            return _objInstance ?? (_objInstance = container.CreateObjectGraph(binding));
        }
    }
}
