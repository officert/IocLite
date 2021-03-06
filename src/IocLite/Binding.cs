﻿using System;
using IocLite.Interfaces;

namespace IocLite
{
    public class Binding : IBinding
    {
        public Type ServiceType { get; set; }
        public Type PluginType { get; set; }
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

        public Binding(Type serviceType, Type pluginType, object instance = null, string name = null)
        {
            if (serviceType != null) ServiceType = serviceType;
            if (pluginType != null) PluginType = pluginType;
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
