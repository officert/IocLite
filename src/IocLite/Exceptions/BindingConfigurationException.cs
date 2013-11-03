
using System;

namespace IocLite.Exceptions
{
    public class BindingConfigurationException : Exception
    {
        public BindingConfigurationException(string message)
            : base(message)
        {
        }

        public BindingConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
