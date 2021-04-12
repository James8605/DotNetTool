using System;
using System.Reflection;

namespace XGS.James.Tool
{
    public static class GenericHelper
    {
        public static MethodInfo GetGenericMethod(string full_class_name, string method_name, string full_type_name)
        {
            MethodInfo method = Type.GetType(full_class_name).GetMethod(method_name);

            return method.MakeGenericMethod(Type.GetType(full_type_name));
        }
    }
}
