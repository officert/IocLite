using System;
using IocLite.Interfaces;

namespace IocLite
{
    public class Binding : IBinding
    {
        public Type PluginType { get; set; }
        public Type ServiceType { get; set; }
        public string Name { get; set; }
        public object Instance { get; set; }
        public ObjectScope ObjectScope { get; set; }

        private bool _disposed;

        ~Binding()
        {
            Dispose(false);
        }

        public Binding()
        {
            ObjectScope = ObjectScope.Transient;
        }

        public Binding(Type pluginType, Type serviceType, object instance = null, string name = null)
        {
            if (pluginType != null) PluginType = pluginType;
            if (serviceType != null) ServiceType = serviceType;
            if (instance != null) Instance = instance;
            if (!string.IsNullOrEmpty(name)) Name = name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
    }
}
