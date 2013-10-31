using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using IocLite.Extensions;
using IocLite.Interfaces;
using IocLite.ObjectFactories;
using IocLite.Resources;

namespace IocLite
{
    public class Container : IContainer
    {
        internal readonly ConcurrentBag<BindingRegistration> BindingRegistrations;

        private readonly object _objetFactoryLock = new object();

        private IEnumerable<IRegistry> _registries;

        public Container()
        {
            BindingRegistrations = new ConcurrentBag<BindingRegistration>();
        }

        public void Register(IList<IRegistry> registries)
        {
            Ensure.ArgumentIsNotNull(registries, "registries");

            _registries = registries;

            foreach (var registry in _registries)
            {
                registry.Load();

                foreach (var binding in registry.Bindings)
                {
                    Ensure.ArgumentIsNotNull(binding, "binding");
                    Ensure.ArgumentIsNotNull(binding.ObjectScope, "binding.ObjectScope");

                    BindingRegistrations.Add(new BindingRegistration
                    {
                        Binding = binding,
                        ObjectFactory = GetObjectFactory(binding.ObjectScope, binding.Instance)
                    });
                }
            }
        }

        public object Resolve(Type type)
        {
            return ResolveInstance(type);
        }

        public T Resolve<T>()
        {
            var type = typeof(T);
            return (T)ResolveInstance(type);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            throw new NotImplementedException();
        }

        public object TryResolve(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            try
            {
                return ResolveInstance(type);
            }
            catch (Exception ex)    //TODO: catch more specific exceptions here
            {
                return null;
            }
        }

        public void Release(Type/**/ type)
        {
            //TODO: need to figure out what exactly Release should do - not sure the current behaviour is right

            Ensure.ArgumentIsNotNull(type, "type");

            var binding = FindBindingRegistrations(type).FirstOrDefault();

            if (binding == null) return;

            binding.Binding.Instance = null;

            //binding.Dispose();
        }

        internal object CreateObjectGraph(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            if (type.IsAbstract || type.IsInterface)
                throw new InvalidOperationException(string.Format(Exceptions.CannotCreateInstanceOfAbstractType, type));

            var constructors = type.GetConstructors();
            var ctor = constructors.FirstOrDefault();   //TODO: need better algorithm for choosing the constructor to use - should be something like
            //TODO: whichever constructor we can resolve the most dependencies for

            if (type.HasADefaultConstructor() || ctor == null)
                return Activator.CreateInstance(type);

            var constructorArgs = ctor.GetParameters().ToList();
            var argObjs = new List<object>();

            foreach (var constructorArg in constructorArgs)
            {
                argObjs.Add(Resolve(constructorArg.ParameterType));
            }
            return Activator.CreateInstance(type, argObjs.ToArray());
        }

        #region Private Helpers

        private object ResolveInstance(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            var registrations = FindBindingRegistrations(type);

            if (registrations == null || !registrations.Any()) return CreateObjectGraph(type);

            //TODO: for now if they registration an interface multiple times throw an exception because we don't have a way to determine the default binding.
            //TODO: need a way to determine the default binding for a PluginType
            if (registrations.Count() > 1) throw new InvalidOperationException(string.Format("Cannot determine the default registration for Plugintype '{0}'", type));

            var reg = registrations.FirstOrDefault();

            lock (_objetFactoryLock)
            {
                return reg.ObjectFactory.GetObject(reg.Binding, this);
            }
        }

        private IEnumerable<BindingRegistration> FindBindingRegistrations(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            if (type == null) return null;

            if (type.IsInterface)
            {
                //interfaces can have multiple implementations - so it can have multiple bindings.
                return BindingRegistrations.Where(x => x.Binding.ServiceType == type);
            }

            var registrations = BindingRegistrations.Where(x => x.Binding.PluginType == type);

            if (registrations == null || !registrations.Any()) return null;

            if (registrations.Count() > 1)
                throw new InvalidOperationException("You cannot have multple bindings for a concerete implementation.");

            return new List<BindingRegistration>
            {
                registrations.FirstOrDefault()
            };
        }

        private IObjectFactory GetObjectFactory(ObjectScope objectScope, object instance = null)
        {
            Ensure.ArgumentIsNotNull(objectScope, "objectScope");

            if (instance != null && objectScope != ObjectScope.Singleton)
                throw new InvalidOperationException("Can only register a type with an instance in singleton scope.");

            switch (objectScope)
            {
                case ObjectScope.Default:
                    return new MultiInstanceObjectFactory();

                case ObjectScope.Singleton:
                    return new SingletonInstanceObjectFactory(instance);

                case ObjectScope.ThreadScope:
                    return new ThreadInstanceObjectFactory();

                case ObjectScope.HttpRequest:
                    return new HttpRequestInstanceObjectFactory();

                default:
                    return new MultiInstanceObjectFactory();
            }
        }

        #endregion
    }
}
