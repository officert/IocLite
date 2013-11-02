using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using IocLite.Extensions;
using IocLite.Interfaces;
using IocLite.ObjectFactories;

namespace IocLite
{
    public class Container : IContainer
    {
        internal readonly ConcurrentBag<BindingRegistration> BindingRegistrations;
        private readonly IBindingResolver _bindingResolver;

        private readonly object _objetFactoryLock = new object();

        public Container()
        {
            BindingRegistrations = new ConcurrentBag<BindingRegistration>();
            _bindingResolver = new BindingResolver();
        }

        public void Register(IList<IRegistry> registries)
        {
            Ensure.ArgumentIsNotNull(registries, "registries");

            foreach (var registry in registries)
            {
                registry.Load();

                foreach (var binding in registry.Bindings)
                {
                    Ensure.ArgumentIsNotNull(binding, "binding");
                    Ensure.ArgumentIsNotNull(binding.ObjectScope, "binding.ObjectScope");

                    ValidateBindings(binding);

                    BindingRegistrations.Add(new BindingRegistration
                    {
                        Binding = binding,
                        ObjectFactory = GetObjectFactory(binding.ObjectScope, binding.Instance)
                    });
                }
            }
        }

        private void ValidateBindings(IBinding binding)
        {
            if (binding.Instance == null && (binding.PluginType.IsAnAbstraction()))
                //if an instance is provided, the plugin type CANNOT be abstract
                throw new InvalidOperationException(string.Format(Resources.Exceptions.CannotUseAnAbstractTypeForAPluginType,
                    binding.PluginType, binding.ServiceType));

            if (BindingRegistrations.Any(x => x.Binding.ServiceType == binding.ServiceType && x.Binding.PluginType == binding.PluginType))
                throw new InvalidOperationException(string.Format(Resources.Exceptions.CannotHaveMultipleBindingsForSameServiceAndPluginType, binding.ServiceType, binding.PluginType));
        }

        public object Resolve(Type type)
        {
            return ResolveInstanceOfService(type);
        }

        public object Resolve<TService>()
        {
            var type = typeof(TService);
            return ResolveInstanceOfService(type);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            //var registrations = FindBindingRegistrations(type);

            //if (registrations == null || !registrations.Any()) return CreateObjectGraph(type);

            //return registrations.f

            throw new NotImplementedException();
        }

        public object TryResolve(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            try
            {
                return ResolveInstanceOfService(type);
            }
            catch (Exception ex)    //TODO: catch more specific exceptions here
            {
                return null;
            }
        }

        //public void Release(Type type)
        //{
        //    //TODO: need to figure out what exactly Release should do - not sure the current behaviour is right

        //    Ensure.ArgumentIsNotNull(type, "type");

        //    var binding = FindBindingRegistrations(type).FirstOrDefault();

        //    if (binding == null) return;

        //    binding.Binding.Instance = null;

        //    //binding.Dispose();
        //}

        internal object CreateObjectGraph(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            if (type.IsAbstract || type.IsInterface)
                throw new InvalidOperationException(string.Format(Resources.Exceptions.CannotCreateInstanceOfAbstractType, type));

            var constructors = type.GetConstructors();
            var ctor = constructors.FirstOrDefault();

            //TODO: need better algorithm for choosing the constructor to use - should be something like
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

        private object ResolveInstanceOfService(Type service)
        {
            Ensure.ArgumentIsNotNull(service, "type");

            var registrations = _bindingResolver.ResolveBindings(service, BindingRegistrations);

            if (registrations == null || !registrations.Any()) return CreateObjectGraph(service);

            //TODO: for now if they registration an interface multiple times throw an exception because we don't have a way to determine the default binding.
            //TODO: need a way to determine the default binding for a PluginType
            if (registrations.Count() > 1) throw new InvalidOperationException(string.Format("Cannot determine the default binding for Plugintype '{0}'", service));

            var reg = registrations.FirstOrDefault();

            lock (_objetFactoryLock)
            {
                return reg.ObjectFactory.GetObject(reg.Binding, this);
            }
        }

        private IObjectFactory GetObjectFactory(ObjectScope objectScope, object instance = null)
        {
            Ensure.ArgumentIsNotNull(objectScope, "objectScope");

            if (instance != null && objectScope != ObjectScope.Singleton)
                throw new InvalidOperationException("Can only register a type with an instance in singleton scope.");

            switch (objectScope)
            {
                case ObjectScope.Transient:
                    return new TransientInstanceObjectFactory();

                case ObjectScope.Singleton:
                    return new SingletonInstanceObjectFactory(instance);

                case ObjectScope.ThreadScope:
                    return new ThreadInstanceObjectFactory();

                case ObjectScope.HttpRequest:
                    return new HttpRequestInstanceObjectFactory();

                default:
                    return new TransientInstanceObjectFactory();
            }
        }

        #endregion
    }
}
