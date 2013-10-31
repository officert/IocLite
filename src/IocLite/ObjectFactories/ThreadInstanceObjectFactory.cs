using System;
using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class ThreadInstanceObjectFactory : IObjectFactory
    {
        [ThreadStatic] 
        private static object _objInstance;

        public ThreadInstanceObjectFactory()
        {
        }

        public ThreadInstanceObjectFactory(object instance)
        {
            _objInstance = instance;
        }

        public object GetObject(IBinding binding, Container container)
        {
            return _objInstance ?? (_objInstance = container.CreateObjectGraph(binding.PluginType));
        }
    }
}
