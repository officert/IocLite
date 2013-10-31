using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using IocLite.Interfaces;
using NUnit.Framework;
using SharpTestsEx;

namespace IocLite.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class ContainerFixture
    {
        private Container _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
        }

        [SetUp]
        public void Setup()
        {
            _container = new Container();
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
        public void Register_RegistryDependencyMapDoesNotSpecifyScope_TypesAreRegisteredInDefaultScope()
        {
            //arrange + act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithoutSpecifiedScope()
            });

            //assert
            foreach (var registration in _container.BindingRegistrations)
            {
                registration.Binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Default);
            }
        }

        [Test]
        public void Register_RegistryDependencyMapSpecifiesSingletonScope_TypesAreRegisteredInSingletonScope()
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
        public void Register_RegistryDependencyMapProvidesInstanceWithoutSpecifyingScope_TypesAreRegisteredInSingletonScope()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            //act
            _container.Register(new List<IRegistry>
            {
                new RegistryWithInstance(currentAssembly)
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
        public void Register_RegistryDependencyMapProvidesInstanceInNonSingletonScope_ThrowsException()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            //act + assert
            Assert.That(() => _container.Register(new List<IRegistry>
            {
                new RegistryWithInstanceInDefaultScope(currentAssembly)
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)));
        }

        #endregion

        #region Resolve(Type)

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

        #region Default Scope Tests

        [Test]
        public void Resolve_RegisteredAsDefault_ResolvesDifferentInstancesOfType_SingleThread()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithDefault()
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
                new RegistryWithDefault()
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
                new RegistryWithInstance(currentAssembly)
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

    internal class RegistryWithDefault : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InDefaultScope();
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

    internal class RegistryWithInstance : Registry
    {
        private readonly Assembly _currentAssembly;

        public RegistryWithInstance(Assembly currentAssembly)
        {
            _currentAssembly = currentAssembly;
        }

        public override void Load()
        {
            For<Assembly>().Use(_currentAssembly);
        }
    }

    internal class RegistryWithInstanceInDefaultScope : Registry
    {
        private readonly Assembly _currentAssembly;

        public RegistryWithInstanceInDefaultScope(Assembly currentAssembly)
        {
            _currentAssembly = currentAssembly;
        }

        public override void Load()
        {
            For<Assembly>().Use(_currentAssembly).InDefaultScope();
        }
    }

    /// <summary>
    /// Type with no dependencies.
    /// </summary>
    internal class TypeWithDefaultConstructor
    {
        public string Foobar { get; set; }

        public TypeWithDefaultConstructor()
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
}
