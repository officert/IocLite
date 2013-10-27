using IocLite.Interfaces;

namespace IocLite.ObjectFactories
{
    public interface IObjectFactory
    {
        object GetObject(IBinding binding, Container container);
    }
}
