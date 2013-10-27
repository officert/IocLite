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
            _container = new Container();
        }

        [SetUp]
        public void Setup()
        {
        }

        #region Resolve(Type)

        [Test]
        public void Resolve_WhenGivenAnInstanceToResolve_DidNotRegisterType_TypeHasDefaultConstructor_ResolvesType()
        {
            //arrange
            var typeToResolve = typeof (TypeWithDefaultConstructor);
            
            //act
            var instance = _container.Resolve(typeToResolve);

            //assert
            instance.Should().Not.Be.Null();
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

    internal class TypeWithDefaultConstructor
    {
        public TypeWithDefaultConstructor()
        {
            
        }
    }
}
