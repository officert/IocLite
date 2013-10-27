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
        private readonly IObjectFactory _multiInstnaceObjectFactory;

        public Container()
        {
            _bindingRegistrations = new ConcurrentDictionary<IBinding, IObjectFactory>();

            _multiInstnaceObjectFactory = new MultiInstanceObjectFactory();
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
            throw new NotImplementedException();
        }

        public object TryResolve(Type type)
        {
            throw new NotImplementedException();
        }

        public void Release(Type type)
        {
            var binding = FindBindings(type).FirstOrDefault();

            if (binding == null) return;

            binding.Instance = null;

            //binding.Dispose();
        }

        public void RegisterBinding(IBinding binding)
        {
            if (binding == null) throw new ArgumentException("binding");

            var success = _bindingRegistrations.TryAdd(binding, GetObjectFactory(binding.ObjectScope, binding.Instance));
        }

        #region Private Helpers

        private object ResolveInstance(Type type)
        {
            if (type.IsInterface)   //TODO; should this be type.IsAbstract || type.IsInterface ??
            {
                var binding = FindBindings(type).FirstOrDefault();

                if (binding == null)
                {
                    var newBinding = new Binding
                    {
                        ConcreteType = type,
                        ObjectScope = ObjectScope.Transient
                    };

                    _bindingRegistrations.TryAdd(newBinding, GetObjectFactory(ObjectScope.Transient));

                    binding = newBinding;
                }

                var objectFactory = _bindingRegistrations[binding];

                return binding.Instance ?? objectFactory.GetObject(binding, this);
            }

            return CreateObjectGraph(type);
        }

        private IObjectFactory GetObjectFactory(ObjectScope objectScope, object instance = null)
        {
            switch (objectScope)
            {
                case ObjectScope.Transient:
                    return _multiInstnaceObjectFactory;

                case ObjectScope.Singleton:
                    return new SingletonObjectFactory(instance);

                default:
                    return _multiInstnaceObjectFactory;
            }
        }

        private IEnumerable<IBinding> FindBindings(Type type)
        {
            if (type == null) return null;

            if (type.IsInterface)
            {
                return _bindingRegistrations.Keys.Where(x => x.AbstractType == type);
            }

            return _bindingRegistrations.Keys.Where(x => x.ConcreteType == type);
        }

        internal object CreateObjectGraph(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsInterface)
                //if (type.IsAbstract || type.IsInterface)
                throw new InvalidOperationException(string.Format("No map for abstract type '{0}' exists. You must register a map with a concrete implementation to inject this interface.", type));

            var constructors = type.GetConstructors();
            var ctor = constructors.FirstOrDefault();

            if (type.HasADefaultConstructor() || ctor == null) //TODO: should no constructor just create the instance, since there are no dependencies to resolve??
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
