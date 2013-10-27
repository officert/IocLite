using System;

namespace IocLite.Interfaces
{
    public interface IBinding : IDisposable
    {
        Type AbstractType { get; set; }
        Type ConcreteType { get; set; }
        string Name { get; set; }
        object Instance { get; set; }
        ObjectScope ObjectScope { get; set; }
    }
}
