using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IocLite.Exceptions;
using IocLite.Extensions;
using IocLite.Interfaces;
using IocLite.ObjectFactories;
using IocLite.Resources;

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

                    ValidateBinding(binding);

                    BindingRegistrations.Add(new BindingRegistration
                    {
                        Binding = binding,
                        ObjectFactory = GetObjectFactory(binding.ObjectScope, binding.Instance)
                    });
                }
            }
        }

        public object Resolve(Type type, string name = null)
        {
            return ResolveInstanceOfService(type, name);
        }

        public object Resolve<TService>(string name = null)
        {
            var type = typeof(TService);
            return ResolveInstanceOfService(type, name);
        }

        public IEnumerable<object> ResolveAll(Type service)
        {
            Ensure.ArgumentIsNotNull(service, "type");

            var registrations = _bindingResolver.ResolveBindings(service, BindingRegistrations);


            throw new NotImplementedException();
        }

        public object TryResolve(Type service)
        {
            Ensure.ArgumentIsNotNull(service, "service");

            try
            {
                return ResolveInstanceOfService(service);
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

            if (type.IsAnAbstraction())
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateInstanceOfAbstractType, type));

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

        private object ResolveInstanceOfService(Type service, string name = null)
        {
            Ensure.ArgumentIsNotNull(service, "type");

            var registrations = _bindingResolver.ResolveBindings(service, BindingRegistrations).ToList();

            if (service.IsAnAbstraction() && !registrations.Any()) throw new BindingConfigurationException(string.Format(ExceptionMessages.CannotResolveAbstractServiceTypeWithNoBinding, service));

            if (!registrations.Any()) return CreateObjectGraph(service);

            if (!string.IsNullOrEmpty(name))
            {
                var namedRegistrations = registrations.Where(x => x.Binding.Name == name).ToList();

                if (!namedRegistrations.Any()) throw new BindingConfigurationException(string.Format(ExceptionMessages.UnknownNamedService, service.Name, name));
                //This should never happen (multiple bindings with the same name), because we are checking for this at registration time
                if (namedRegistrations.Count() > 1) throw new BindingConfigurationException(string.Format(ExceptionMessages.MultipleNamedBindingsFoundForServiceWithSameName, service, name));

                var namedRegistration = registrations.FirstOrDefault();

                return GetObjectFromObjectFactory(namedRegistration.Binding, namedRegistration.ObjectFactory);
            }

            //This should never happen (multiple default bindings), because we are checking for this at registration time
            if (registrations.Count() > 1)
                throw new BindingConfigurationException(string.Format(ExceptionMessages.MultipleDefaultBindingsFoundForService, service));

            var registration = registrations.FirstOrDefault();

            return GetObjectFromObjectFactory(registration.Binding, registration.ObjectFactory);
        }

        private object GetObjectFromObjectFactory(IBinding binding, IObjectFactory objectFactory)
        {
            lock (_objetFactoryLock)
            {
                return objectFactory.GetObject(binding, this);
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

        private void ValidateBinding(IBinding binding)
        {
            CheckThatIfInstanceIsProvidedThePluginTypeIsNotAnAbstractType(binding);
            CheckIfAnotherBindingAlreadyExistsForTheServiceType(binding, BindingRegistrations);
        }

        /// <summary>
        /// Checks that if an instance is provided the plugin type is not an abstract type.
        /// A Plugin type cannot not be abstract because abstract types cannot be instantiated.
        /// A Plugin type can ONLY be abstract if the Instance property has a value - because we won't try and instantiate the Plugin Type if an Instance already exists.
        /// </summary>
        public static void CheckThatIfInstanceIsProvidedThePluginTypeIsNotAnAbstractType(IBinding binding)
        {
            if (binding.Instance == null && (binding.PluginType.IsAnAbstraction()))
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotUseAnAbstractTypeForAPluginType, binding.PluginType, binding.ServiceType));
        }

        /// <summary>
        /// Checks that another binding hasn't already been registered for the same service.
        /// If another binding exists for the same service an exception will be thrown, unless the bindings are named.
        /// If multiple bindings exist for the same service we cannot know which is the default binding, so they must be named.
        /// </summary>
        public static void CheckIfAnotherBindingAlreadyExistsForTheServiceType(IBinding binding, IEnumerable<BindingRegistration> bindingRegistrations)
        {
            var otherBindingsForSameService = bindingRegistrations.Where(x => x.Binding.ServiceType == binding.ServiceType).ToList();

            if (!otherBindingsForSameService.Any()) return;

            if ((string.IsNullOrEmpty(binding.Name) && otherBindingsForSameService.Any(x => string.IsNullOrEmpty(x.Binding.Name))))
                ThrowDetailedBindingConfigurationException(binding, otherBindingsForSameService, ExceptionMessages.CannotHaveMultipleDefaultBindingsForService);

            if (!string.IsNullOrEmpty(binding.Name) && otherBindingsForSameService.Any(x => x.Binding.Name == binding.Name))
                ThrowDetailedBindingConfigurationException(binding, otherBindingsForSameService, ExceptionMessages.CannotHaveMultipleNamedBindingsForServiceWithSameName, binding.Name);
        }

        private static void ThrowDetailedBindingConfigurationException(IBinding binding,
            IEnumerable<BindingRegistration> otherBindingsForSameService,
            string errorMessage,
            string bindingName = null)
        {
            const string errorMessageFormatWithNewline = "For<{0}>().Use<{1}>(); {2}";
            const string errorMessageFormatWithNewlineNamed = "For<{0}>().Use<{1}>().Named('{3}'); {2}";
            const string errorMessageFormat = "For<{0}>().Use<{1}>();";
            const string errorMessageFormatNamed = "For<{0}>().Use<{1}>().Named('{2}');";

            var sb = new StringBuilder();

            foreach (var bindingRegistration in otherBindingsForSameService)
            {
                sb.Append(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessageFormatWithNewline, bindingRegistration.Binding.ServiceType, bindingRegistration.Binding.PluginType, Environment.NewLine)
                    : string.Format(errorMessageFormatWithNewlineNamed, bindingRegistration.Binding.ServiceType, bindingRegistration.Binding.PluginType, Environment.NewLine, bindingName));
            }
            sb.Append(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessageFormat, binding.ServiceType, binding.PluginType)
                    : string.Format(errorMessageFormatNamed, binding.ServiceType, binding.PluginType, bindingName));

            throw new BindingConfigurationException(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessage, binding.ServiceType, Environment.NewLine, sb)
                    : string.Format(errorMessage, binding.ServiceType, bindingName, Environment.NewLine, sb));
        }

        #endregion
    }
}
