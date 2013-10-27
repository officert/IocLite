using System;

namespace IocLite.Interfaces
{
    public interface IBinding : IDisposable
    {
        Type PluginType { get; set; }
        Type ServiceType { get; set; }
        string Name { get; set; }
        object Instance { get; set; }
        ObjectScope ObjectScope { get; set; }
    }
}
