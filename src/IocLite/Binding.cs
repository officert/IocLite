using System;
using IocLite.Interfaces;

namespace IocLite
{
    public class Binding : IBinding
    {
        public Type AbstractType { get; set; }
        public Type ConcreteType { get; set; }
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

        public Binding(Type abstractType, Type concreteType, object instance = null, string name = null)
        {
            if (abstractType != null) AbstractType = abstractType;
            if (concreteType != null) ConcreteType = concreteType;
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
