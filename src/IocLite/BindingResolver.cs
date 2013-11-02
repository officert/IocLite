using System;
using System.Collections.Generic;
using System.Linq;
using IocLite.Interfaces;

namespace IocLite
{
    public class BindingResolver : IBindingResolver
    {
        public IEnumerable<BindingRegistration> ResolveBindings(Type service, IEnumerable<BindingRegistration> bindingRegistrations)
        {
            Ensure.ArgumentIsNotNull(service, "service");

            return bindingRegistrations.Where(x => x.Binding.ServiceType == service);
        }
    }
}
