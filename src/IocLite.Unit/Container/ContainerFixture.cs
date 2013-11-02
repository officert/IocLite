﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using IocLite.Interfaces;
using IocLite.Resources;
using NUnit.Framework;
using SharpTestsEx;

namespace IocLite.Unit.Container
{
    [TestFixture]
    [Category("Unit")]
    public class ContainerFixture
    {
        private IocLite.Container _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
        }

        [SetUp]
        public void Setup()
        {
            _container = new IocLite.Container();
        }

        #region Register

        [Test]
        public void Register_RegistriesIsNull_ThrowsException()
        {
            //arrange
            List<IRegistry> registries = null;

            //act + assert
            Assert.That(() => _container.Register(registries),
                Throws.Exception.TypeOf(typeof(ArgumentNullException)).With.Message.StartsWith(Ensure.ArgumentNullMessage));
        }

        [Test]
        public void Register_DependencyMapDoesNotSpecifyScope_TypesAreRegisteredInDefaultScope()
        {
            //arrange + act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithoutSpecifiedScope()
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Transient);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesSingletonScope_TypesAreRegisteredInSingletonScope()
        {
            //arrange + act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithSingleton()
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Singleton);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesThreadScope_TypesAreRegisteredInThreadScope()
        {
            //arrange + act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithThreaded()
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.ThreadScope);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesHttpRequestScope_TypesAreRegisteredInHttpRequestScope()
        {
            //arrange + act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithHttpRequest()
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.HttpRequest);
            }
        }

        [Test]
        public void Register_DependencyMapProvidesInstanceWithoutSpecifyingScope_TypesAreRegisteredInSingletonScope()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            //act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithAbstractTypeInstance(currentAssembly)
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.Instance.Should().Not.Be.Null();
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Singleton);
            }
        }

        /// <summary>
        /// Cannot register a type with an instance in anything but singleton scope, which is the default scope when an instance is provided
        /// </summary>
        [Test]
        public void Register_DependencyMapProvidesInstanceInNonSingletonScope_ThrowsException()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithInstanceInTransientScope(currentAssembly)
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)));
        }

        /// <summary>
        /// Cannot register an interface with a plugin type that is also an interface
        /// </summary>
        [Test]
        public void Register_DependencyMapWithInterfaceMappedToInterface_ThrowsException()
        {
            //arrange

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithInterfaceServiceTypeAndInterfacePluginType()
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo(string.Format(Resources.Exceptions.CannotUseAnAbstractTypeForAPluginType, typeof(ITypeWithDefaultConstructor), typeof(ITypeWithDefaultConstructor))));
        }

        /// <summary>
        /// Cannot register an abstract class with a plug in type that is also an abstract type
        /// </summary>
        [Test]
        public void Register_DependencyMapWithAbstractTypeMappedToAbstractType_ThrowsException()
        {
            //arrange

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithAbstractServiceTypeAndAbstractPluginType()
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo(string.Format(Resources.Exceptions.CannotUseAnAbstractTypeForAPluginType, typeof(BaseTypeWithDefaultConstructor), typeof(BaseTypeWithDefaultConstructor))));
        }

        /// <summary>
        /// Cannot register an abstract class with a plug in type that is an interface
        /// </summary>
        [Test]
        public void Register_DependencyMapWithAbstractTypeMappedToInterface_ThrowsException()
        {
            //arrange

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithInterfaceServiceTypeAndAbstractPluginType()
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo(string.Format(Resources.Exceptions.CannotUseAnAbstractTypeForAPluginType, typeof(BaseTypeWithDefaultConstructor), typeof(ITypeWithDefaultConstructor))));
        }

        /// <summary>
        /// Cannot register the same map (same service to same plugin) multiple times
        /// </summary>
        [Test]
        public void Register_DependencyMapWithMultipleMapsUsingSameType_TypeIsConcrete_ThrowsException()
        {
            //arrange

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithMultipleMapsUsingSameConcreteType()
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo(string.Format(Resources.Exceptions.CannotHaveMultipleBindingsForSameServiceAndPluginType, typeof(TypeWithDefaultConstructor), typeof(TypeWithDefaultConstructor))));
        }

        [Test]
        [Ignore("Need to figure out how to deal with multiple bindings for a plugin type")]
        public void Register_AllowsMultipleMapsWithSameServiceType_DifferentPluginImplementations()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithMultipleMapsUsingSameServiceTypeButDifferentPluginType()
            });

            //act
            var instance = _container.Resolve<ITypeWithDefaultConstructor>();

            //assert
        }

        #endregion

        #region Resolve

        [Test]
        public void Resolve_DidNotRegisterType_ResolvesNewInstanceEveryTime()
        {
            //arrange
            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            var instance1 = _container.Resolve(typeToResolve);
            var instance2 = _container.Resolve(typeToResolve);
            var instance3 = _container.Resolve(typeToResolve);

            //assert
            instance1.Should().Not.Be.SameInstanceAs(instance2);
            instance2.Should().Not.Be.SameInstanceAs(instance3);
            instance3.Should().Not.Be.SameInstanceAs(instance1);
        }

        [Test]
        public void Resolve_DidNotRegisterType_AndTypeHasDefaultConstructor_ResolvesType()
        {
            //arrange
            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            var instance = _container.Resolve(typeToResolve);

            //assert
            instance.Should().Not.Be.Null();
        }

        [Test]
        public void Resolve_DidNotRegisterType_AndTypeHasADependencyInConstructor_DependencyHasDefaultConstructor_ResolvesType()
        {
            //arrange
            var typeToResolve = typeof(TypeWith1DependencyInConstructor);

            //act
            var instance = _container.Resolve(typeToResolve);

            //assert
            instance.Should().Not.Be.Null();
            var castInstance = (TypeWith1DependencyInConstructor)instance;
            castInstance.TypeWithDefaultConstructor.Should().Not.Be.Null();
        }

        [Test]
        public void Resolve_DidNotRegisterType_AndTypeIsAnInterface_ThrowsException()
        {
            //arrange
            var typeToResolve = typeof (ITypeWithDefaultConstructor);

            //act + assert
            Assert.That(() => _container.Resolve(typeToResolve),
                Throws.InvalidOperationException.With.Message.EqualTo(string.Format(Resources.Exceptions.CannotCreateInstanceOfAbstractType, typeToResolve)));
        }

        [Test]
        public void Resolve_DidNotRegisterType_AndTypeIsAnAbstractType_ThrowsException()
        {
            //arrange
            var typeToResolve = typeof(Assembly);

            //act + assert
            Assert.That(() => _container.Resolve(typeToResolve),
                Throws.InvalidOperationException.With.Message.EqualTo(string.Format(Exceptions.CannotCreateInstanceOfAbstractType, typeToResolve)));
        }

        #region Default Scope Tests

        [Test]
        public void Resolve_RegisteredAsDefault_ResolvesDifferentInstancesOfType_SingleThread()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithTransient()
            });

            const string propValue1 = "string1";
            const string propValue2 = "string2";

            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            var instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance1.Foobar = propValue1;

            var instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance2.Foobar = propValue2;

            //assert
            instance1.Foobar.Should().Be.EqualTo(propValue1);
            instance2.Foobar.Should().Be.EqualTo(propValue2);
        }

        [Test]
        public void Resolve_RegisteredAsDefault_ResolvesDifferentInstancesOfType_MultipleThreads()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithTransient()
            });

            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            TypeWithDefaultConstructor instance1 = null;
            TypeWithDefaultConstructor instance2 = null;

            var thread1 = new Thread(x =>
            {
                instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            });
            var thread2 = new Thread(x =>
            {
                instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            //assert
            instance1.Should().Not.Be.SameInstanceAs(instance2);
        }

        #endregion

        #region Singleton Scope Tests

        [Test]
        public void Resolve_RegisteredAsSingleton_ResolvesSameInstanceOfType_SingleThread()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithSingleton()
            });

            const string propValue1 = "string1";
            const string propValue2 = "string2";

            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            var instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance1.Foobar = propValue1;

            var instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance2.Foobar = propValue2;

            //assert
            instance1.Should().Be.SameInstanceAs(instance2);

            instance1.Foobar.Should().Be.EqualTo(propValue2);
            instance2.Foobar.Should().Be.EqualTo(propValue2);
        }

        [Test]
        public void Resolve_RegisteredAsSingleton_ResolvesSameInstanceOfType_MultipleThreads()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithSingleton()
            });
            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            TypeWithDefaultConstructor instance1 = null;
            TypeWithDefaultConstructor instance2 = null;

            var thread1 = new Thread(x =>
            {
                instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance1.Foobar = "string 1";
            });
            var thread2 = new Thread(x =>
            {
                instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance2.Foobar = "string 2";
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            //assert
            instance1.Should().Be.SameInstanceAs(instance2);
            instance1.Foobar.Should().Be.EqualTo(instance2.Foobar);
        }

        [Test]
        public void Resolve_RegisteredWithInstance_ResolvesWithInstanceProvidedWhenRegistered_SingleThread()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            _container.Register(new List<IRegistry>
            {
                new RegistryWithAbstractTypeInstance(currentAssembly)
            });

            //act
            var instance = _container.Resolve(typeof(Assembly));

            //assert
            currentAssembly.Should().Be.SameInstanceAs(instance);
        }

        #endregion

        #region Thread Scope Tests

        [Test]
        public void Resolve_RegisteredAsThreaded_ResolvesSameInstanceOfType_SingleThread()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithThreaded()
            });
            var typeToResolve = typeof(TypeWithDefaultConstructor);


            //act
            var instance1 = _container.Resolve(typeToResolve);
            var instance2 = _container.Resolve(typeToResolve);

            //assert
            instance1.Should().Be.SameInstanceAs(instance2);
        }

        [Test]
        public void Resolve_RegisteredAsThreaded_ResolvesDifferentInstancesOfType_MultipleThreads()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithThreaded()
            });
            var typeToResolve = typeof(TypeWithDefaultConstructor);


            //act
            TypeWithDefaultConstructor instance1_thread1 = null;
            TypeWithDefaultConstructor instance2_thread1 = null;
            TypeWithDefaultConstructor instance3_thread1 = null;

            TypeWithDefaultConstructor instance1_thread2 = null;
            TypeWithDefaultConstructor instance2_thread2 = null;
            TypeWithDefaultConstructor instance3_thread2 = null;

            var thread1 = new Thread(x =>
            {
                instance1_thread1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance1_thread1.Foobar = "thread1_instance1";

                instance2_thread1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance2_thread1.Foobar = "thread1_instance2";

                instance3_thread1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance3_thread1.Foobar = "thread1_instance3";
            });
            var thread2 = new Thread(x =>
            {
                instance1_thread2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance1_thread2.Foobar = "thread2_instance1";

                instance2_thread2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance2_thread2.Foobar = "thread2_instance2";

                instance3_thread2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                instance3_thread2.Foobar = "thread2_instance3";
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            //assert
            instance1_thread1.Should().Be.SameInstanceAs(instance2_thread1);
            instance2_thread1.Should().Be.SameInstanceAs(instance3_thread1);
            instance3_thread1.Should().Be.SameInstanceAs(instance1_thread1);
            instance1_thread1.Foobar.Should().Be.EqualTo("thread1_instance3");  //last in wins - so it should be the last value Foobar was set to
            instance2_thread1.Foobar.Should().Be.EqualTo("thread1_instance3");
            instance3_thread1.Foobar.Should().Be.EqualTo("thread1_instance3");

            instance1_thread2.Should().Be.SameInstanceAs(instance2_thread2);
            instance2_thread2.Should().Be.SameInstanceAs(instance3_thread2);
            instance3_thread2.Should().Be.SameInstanceAs(instance1_thread2);
            instance1_thread2.Foobar.Should().Be.EqualTo("thread2_instance3");  //last in wins - so it should be the last value Foobar was set to
            instance2_thread2.Foobar.Should().Be.EqualTo("thread2_instance3");
            instance3_thread2.Foobar.Should().Be.EqualTo("thread2_instance3");
        }

        #endregion

        #region Http Request Scope Tests


        #endregion

        #region Constructor Tests

        #endregion

        #endregion

        #region CreateObjectGraph

        [Test]
        public void CreateObjectGraph_TypeIsNull_ThrowsException()
        {
            //arrange
            Type type = null;

            //act + assert
            Assert.That(() => _container.CreateObjectGraph(type),
                Throws.Exception.TypeOf(typeof(ArgumentNullException)).With.Message.StartsWith(Ensure.ArgumentNullMessage));
        }

        #endregion
    }

    internal class RegistryWithoutSpecifiedScope : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithTransient : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InTransientScope();
        }
    }

    internal class RegistryWithSingleton : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InSingletonScope();
        }
    }

    internal class RegistryWithThreaded : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InThreadScope();
        }
    }

    internal class RegistryWithHttpRequest : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InHttpRequestScope();
        }
    }

    internal class RegistryWithAbstractTypeInstance : Registry
    {
        private readonly Assembly _currentAssembly;

        public RegistryWithAbstractTypeInstance(Assembly currentAssembly)
        {
            _currentAssembly = currentAssembly;
        }

        public override void Load()
        {
            For<Assembly>().Use(_currentAssembly);
        }
    }

    internal class RegistryWithConcreteTypeInstance : Registry
    {
        private readonly TypeWithDefaultConstructor _typeWithDefaultConstructor;

        public RegistryWithConcreteTypeInstance(TypeWithDefaultConstructor typeWithDefaultConstructor)
        {
            _typeWithDefaultConstructor = typeWithDefaultConstructor;
        }

        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use(_typeWithDefaultConstructor);
        }
    }

    internal class RegistryWithInstanceInTransientScope : Registry
    {
        private readonly Assembly _currentAssembly;

        public RegistryWithInstanceInTransientScope(Assembly currentAssembly)
        {
            _currentAssembly = currentAssembly;
        }

        public override void Load()
        {
            For<Assembly>().Use(_currentAssembly).InTransientScope();
        }
    }

    internal class RegistryWithMultipleMapsUsingSameConcreteType : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>();
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithInterfaceServiceTypeAndInterfacePluginType : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<ITypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithAbstractServiceTypeAndAbstractPluginType : Registry
    {
        public override void Load()
        {
            For<BaseTypeWithDefaultConstructor>().Use<BaseTypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithInterfaceServiceTypeAndAbstractPluginType : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<BaseTypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithMultipleMapsUsingSameServiceTypeButDifferentPluginType : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>();
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructorAlternateImpl>();
        }
    }

    /// <summary>
    /// Type with no dependencies.
    /// </summary>
    internal class TypeWithDefaultConstructor : BaseTypeWithDefaultConstructor, ITypeWithDefaultConstructor
    {
        public string Foobar { get; set; }

        public TypeWithDefaultConstructor()
        {

        }
    }

    /// <summary>
    /// Type with no dependencies.
    /// </summary>
    internal class TypeWithDefaultConstructorAlternateImpl : BaseTypeWithDefaultConstructor, ITypeWithDefaultConstructor
    {
        public string Foobar { get; set; }

        public TypeWithDefaultConstructorAlternateImpl()
        {

        }
    }

    /// <summary>
    /// This is a type with one dependency, and the one dependency has a default constructor.
    /// </summary>
    internal class TypeWith1DependencyInConstructor
    {
        public readonly TypeWithDefaultConstructor TypeWithDefaultConstructor;

        public TypeWith1DependencyInConstructor(TypeWithDefaultConstructor typeWithDefaultConstructor)
        {
            TypeWithDefaultConstructor = typeWithDefaultConstructor;
        }
    }

    internal interface ITypeWithDefaultConstructor
    {
        
    }

    internal abstract class BaseTypeWithDefaultConstructor : ITypeWithDefaultConstructor
    {
        
    }
}
