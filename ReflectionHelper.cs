using System.Reflection;
using System;

namespace DotNetTool
{
    public class ReflectionHelper
    {
        public static object GetObject(string full_type_name, object[] param = null)
        {
            Assembly assembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
            return assembly.CreateInstance(full_type_name, true,
                BindingFlags.Default, null, param, null, null); //类的完全限定名（即包括命名空间）
        }

        public static MethodInfo GetGenericMethod(string full_class_name, string method_name, string full_type_name,string assembly_path)
        {

            Assembly assembly = Assembly.Load(assembly_path);
            MethodInfo method = assembly.GetType(full_class_name).GetMethod(method_name);

            return method.MakeGenericMethod(Type.GetType(full_type_name));
        }
    }
}
