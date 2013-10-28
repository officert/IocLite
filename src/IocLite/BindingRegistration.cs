using IocLite.Interfaces;

namespace IocLite
{
    public class BindingRegistration
    {
        public IBinding Binding { get; set; }

        public IObjectFactory ObjectFactory { get; set; }
    }
}
