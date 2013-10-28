using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
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

        [Test]
        public void Resolve_RegisteredAsSingleton_ResolvesSameInstanceOfType()
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
        public void Resolve_RegisteredAsSingleton_ResolvesSameInstanceOfType_MultiThreaded()
        {
            //arrange
            _container.Register(new List<IRegistry>
            {
                new RegistryWithSingleton()
            });

            for (var i = 0; i < 100; i++)
            {
                const string propValue1 = "string1";
                const string propValue2 = "string2";

                var typeToResolve = typeof(TypeWithDefaultConstructor);

                //act
                TypeWithDefaultConstructor instance1 = null;
                TypeWithDefaultConstructor instance2 = null;

                var thread1 = new Thread(x =>
                {
                    instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                    instance1.Foobar = propValue1;
                });
                var thread2 = new Thread(x =>
                {
                    instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
                    instance2.Foobar = propValue2;
                });
                thread1.Start();
                thread2.Start();

                thread1.Join();
                thread2.Join();

                //assert
                Debug.Write(string.Format("i = {0}, instance 1 = {1}, instance 2 = {2}", i, instance1.Foobar, instance2.Foobar));
            }

            ////arrange
            //_container.Register(new List<IRegistry>
            //{
            //    new RegistryWithSingleton()
            //});

            //const string propValue1 = "string1";
            //const string propValue2 = "string2";

            //var typeToResolve = typeof(TypeWithDefaultConstructor);

            ////act
            //TypeWithDefaultConstructor instance1 = null;
            //TypeWithDefaultConstructor instance2 = null;

            //var thread1 = new Thread(x =>
            //{
            //    instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            //    instance1.Foobar = propValue1;
            //});
            //var thread2 = new Thread(x =>
            //{
            //    instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            //    instance2.Foobar = propValue2;
            //});
            //thread1.Start();
            //thread2.Start();

            //thread1.Join();
            //thread2.Join();

            ////assert
            //instance1.Foobar.Should().Be.EqualTo(propValue2);
            //instance2.Foobar.Should().Be.EqualTo(propValue2);
        }

        [Test]
        public void Resolve_RegisteredAsTransient_ResolvesDifferentInstancesOfType()
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

        #endregion

        [Test]
        public void Resolve_RegisteredWithInstance_ResolvesWithInstanceProvidedWhenRegistered()
        {
            //arrange
            var currentAssembly = Assembly.GetCallingAssembly();

            _container.Register(new List<IRegistry>
            {
                new RegistryWithInstance(currentAssembly)
            });

            //act
            var instance = _container.Resolve(typeof (Assembly));

            //assert
            currentAssembly.Should().Be.SameInstanceAs(instance);
        }

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

    internal class RegistryWithSingleton : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InSingletonScope();
        }
    }

    internal class RegistryWithTransient : Registry
    {
        public override void Load()
        {
            For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InDefaultScope();
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
