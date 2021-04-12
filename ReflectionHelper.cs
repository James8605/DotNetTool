using System.Reflection;

namespace XGS.James.Tool
{
    public class ReflectionHelper
    {
        public static object GetObject(string full_type_name, object[] param = null)
        {
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            return assembly.CreateInstance(full_type_name, true,
                BindingFlags.Default, null, param, null, null); //类的完全限定名（即包括命名空间）
        }
    }
}
