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
        private readonly ConcurrentDictionary<IBinding, IObjectFactory> _bindingRegistrations;
        private readonly IObjectFactory _multiInstanceObjectFactory;

        public Container()
        {
            _bindingRegistrations = new ConcurrentDictionary<IBinding, IObjectFactory>();

            _multiInstanceObjectFactory = new MultiInstanceObjectFactory();
        }

        public DependencyMap<TAbstractType> For<TAbstractType>()
        {
            return new DependencyMap<TAbstractType>(this);
        }

        public object Resolve(Type type)
        {
            return ResolveInstance(type);
        }

        public object Resolve<T>()
        {
            var type = typeof(T);
            return ResolveInstance(type);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            throw new NotImplementedException();
        }

        public object TryResolve(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            throw new NotImplementedException();
        }

        public void Release(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            var binding = FindBindings(type).FirstOrDefault();

            if (binding == null) return;

            binding.Instance = null;

            //binding.Dispose();
        }

        public void RegisterBinding(IBinding binding)
        {
            Ensure.ArgumentIsNotNull(binding, "binding");

            var success = _bindingRegistrations.TryAdd(binding, GetObjectFactory(binding.ObjectScope, binding.Instance));
        }

        #region Private Helpers

        private object ResolveInstance(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            var binding = FindBindings(type).FirstOrDefault();

            if (binding == null) return CreateObjectGraph(type);

            if (type.IsInterface)   //TODO; should this be type.IsAbstract || type.IsInterface ??
            {
                var objectFactory = _bindingRegistrations[binding];

                return objectFactory.GetObject(binding, this);
            }

            return CreateObjectGraph(type);
        }

        private IObjectFactory GetObjectFactory(ObjectScope objectScope, object instance = null)
        {
            Ensure.ArgumentIsNotNull(objectScope, "objectScope");

            switch (objectScope)
            {
                case ObjectScope.Transient:
                    return _multiInstanceObjectFactory;

                case ObjectScope.Singleton:
                    return new SingletonObjectFactory(instance);

                default:
                    return _multiInstanceObjectFactory;
            }
        }

        private IEnumerable<IBinding> FindBindings(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            if (type == null) return null;

            if (type.IsInterface)
            {
                return _bindingRegistrations.Keys.Where(x => x.AbstractType == type);
            }

            return _bindingRegistrations.Keys.Where(x => x.ConcreteType == type);
        }

        internal object CreateObjectGraph(Type type)
        {
            Ensure.ArgumentIsNotNull(type, "type");

            if (type.IsInterface)
                //if (type.IsAbstract || type.IsInterface)
                throw new InvalidOperationException(string.Format("No map for abstract type '{0}' exists. You must register a map with a concrete implementation to inject this interface.", type));

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

        #endregion
    }
}
