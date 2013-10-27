using System;
using System.Collections.Generic;
using System.Linq;

namespace IocLite
{
    public static class Ensure
    {
        public static void ArgumentIsNotNull(object argument, string name)
        {
            if (argument == null) throw new ArgumentNullException(name, "Cannot be null");
        }

        public static void ArgumentIsNotNullOrEmtpy<T>(ICollection<T> argument, string name) where T : class 
        {
            if (argument == null || !argument.Any()) throw new ArgumentException("Cannot be null or empty", name);
        }

        public static void ArgumentIsNotNullOrEmtpy(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument)) throw new ArgumentException("Cannot be null or empty", name);
        }
    }
}
