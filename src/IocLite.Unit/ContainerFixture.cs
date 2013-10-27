using System;
using NUnit.Framework;
using SharpTestsEx;

namespace IocLite.Unit
{
    [TestFixture]
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

        #region Resolve(Type)

        [Test]
        public void Resolve_DidNotRegisterType_AndTypeHasDefaultConstructor_ResolvesType()
        {
            //arrange
            var typeToResolve = typeof (TypeWithDefaultConstructor);
            
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
            var castInstance = (TypeWith1DependencyInConstructor) instance;
            castInstance.TypeWithDefaultConstructor.Should().Not.Be.Null();
        }

        [Test]
        public void Resolve_RegisteredAsSingleton_ResolvesSameInstanceOfType()
        {
            //arrange
            _container.For<TypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().InSingletonScope();
            const string propValue1 = "string1";
            const string propValue2 = "string2";

            var typeToResolve = typeof(TypeWithDefaultConstructor);

            //act
            var instance1 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance1.Foobar = propValue1;

            var instance2 = (TypeWithDefaultConstructor)_container.Resolve(typeToResolve);
            instance2.Foobar = propValue2;

            //assert
            instance1.Foobar.Should().Be.EqualTo(propValue2);
            instance2.Foobar.Should().Be.EqualTo(propValue2);
        }

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
