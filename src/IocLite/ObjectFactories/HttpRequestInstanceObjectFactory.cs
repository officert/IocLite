using System;
using System.Web;
using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class HttpRequestInstanceObjectFactory : IObjectFactory
    {
        private readonly string _instanceKey = Guid.NewGuid().ToString();

        public HttpRequestInstanceObjectFactory()
        {
        }

        public HttpRequestInstanceObjectFactory(object instance)
        {
            HttpContext.Current.Items[_instanceKey] = instance;
        }

        public object GetObject(IBinding binding, Container container)
        {
            return HttpContext.Current.Items[_instanceKey] ?? (HttpContext.Current.Items[_instanceKey] = container.CreateObjectGraph(binding.PluginType));
        }
    }
}
