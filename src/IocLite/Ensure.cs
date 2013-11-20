using System;
using System.Collections.Generic;
using System.Linq;

namespace IocLite
{
    public static class Ensure
    {
        public static string ArgumentIsNullMessageFormat = ArgumentNullMessage + @"
Parameter name: {0}";
        public static string ArgumentIsNullOrEmptyMessageFormat = ArgumentNullOrEmptyMessage + @"
Parameter name: {0}";

        private const string ArgumentNullMessage = "Cannot be null";
        private const string ArgumentNullOrEmptyMessage = "Cannot be null or empty";

        public static void ArgumentIsNotNull(object argument, string name)
        {
            if (argument == null) throw new ArgumentNullException(name, ArgumentNullMessage);
        }

        public static void ArgumentIsNotNullOrEmtpy<T>(ICollection<T> argument, string name) where T : class 
        {
            if (argument == null || !argument.Any()) throw new ArgumentException(ArgumentNullOrEmptyMessage, name);
        }

        public static void ArgumentIsNotNullOrEmtpy<T>(IEnumerable<T> argument, string name) where T : class
        {
            if (argument == null || !argument.Any()) throw new ArgumentException(ArgumentNullOrEmptyMessage, name);
        }

        public static void ArgumentIsNotNullOrEmtpy(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument)) throw new ArgumentException(ArgumentNullOrEmptyMessage, name);
        }
    }
}
