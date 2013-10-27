using System;

namespace IocLite.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasADefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
