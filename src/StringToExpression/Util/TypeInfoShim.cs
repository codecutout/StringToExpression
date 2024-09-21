using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StringToExpression.Util
{
    internal static class TypeShim
    {
        public static PropertyInfo GetProperty(Type type, string property)
        {
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
            return type.GetRuntimeProperties().FirstOrDefault(x => x.Name.Equals(property, StringComparison.OrdinalIgnoreCase));
#else
            return type.GetTypeInfo().GetProperty(property, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public) 
                ?? throw new Exception($"Unable to find property '{property}' on object of type '{type}'");
#endif
        }
    }
}
