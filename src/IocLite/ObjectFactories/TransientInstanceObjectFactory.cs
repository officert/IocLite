using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public class TransientInstanceObjectFactory : IObjectFactory
    {
        public object GetObject(IBinding binding, Container container)
        {
            return container.CreateObjectGraph(binding.PluginType);
        }
    }
}
