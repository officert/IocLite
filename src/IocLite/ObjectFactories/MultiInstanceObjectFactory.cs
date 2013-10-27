using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class MultiInstanceObjectFactory : IObjectFactory
    {
        public object GetObject(IBinding binding, Container container)
        {
            return container.CreateObjectGraph(binding.ConcreteType);
        }
    }
}
