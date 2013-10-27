namespace IocLite.Interfaces
{
    public interface IObjectFactory
    {
        object GetObject(IBinding binding, Container container);
    }
}
