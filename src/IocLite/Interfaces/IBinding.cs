using System;

namespace IocLite.Interfaces
{
    public interface IBinding : IDisposable
    {
        Type ServiceType { get; set; }
        Type PluginType { get; set; }
        string Name { get; set; }
        object Instance { get; set; }
        ObjectScope ObjectScope { get; set; }
    }
}
