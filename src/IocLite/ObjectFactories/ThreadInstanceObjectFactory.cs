using System;
using System.Threading;
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
            //TODO: figure out whether it's better to use ThreadStatic attribute, or TLS

            var instance = Thread.GetData(Thread.GetNamedDataSlot("foobar"));
            if(instance == null) Thread.SetData(Thread.GetNamedDataSlot("foobar"), container.CreateObjectGraph(binding));

            return Thread.GetData(Thread.GetNamedDataSlot("foobar"));

            //return _objInstance ?? (_objInstance = container.CreateObjectGraph(binding));
        }
    }
}
