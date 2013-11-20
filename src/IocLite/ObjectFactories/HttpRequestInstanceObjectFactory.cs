using System;
using System.Collections;
using System.Web;
using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class HttpRequestInstanceObjectFactory : IObjectFactory
    {
        private readonly string _instanceKey = Guid.NewGuid().ToString();
        private readonly IDictionary _contextItems;

        public HttpRequestInstanceObjectFactory()
        {
            _contextItems = new Hashtable();
        }

        //public HttpRequestInstanceObjectFactory(object instance)
        //{
        //    HttpContext.Current.Items[_instanceKey] = instance;
        //}

        public object GetObject(IBinding binding, Container container)
        {
            var items = HttpContext.Current == null ? _contextItems : HttpContext.Current.Items;

            return items[_instanceKey] ?? (items[_instanceKey] = container.CreateObjectGraph(binding));
        }
    }
}
