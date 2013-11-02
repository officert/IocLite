using System;
using System.Collections.Generic;

namespace IocLite.Interfaces
{
    public interface IBindingResolver
    {
        IEnumerable<BindingRegistration> ResolveBindings(Type service, IEnumerable<BindingRegistration> bindingRegistrations);
    }
}
