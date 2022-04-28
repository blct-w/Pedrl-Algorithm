using System;
using System.Dynamic;
using System.Reflection;

namespace blct
{
    public static class DynamicExtends
    {
        public static T Get<T>(this object objectInstance,string propertyName)
        {
            Type objectType = objectInstance.GetType();
            PropertyInfo pi = objectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi == null)
                throw new ArgumentException("自动设置参数值失败。参数化变量名称" + propertyName + "必须和对象中的属性名称一样。");
            if (!pi.CanRead)
                throw new ArgumentException("自动设置参数值失败。对象的" + propertyName + "属性没有get方式，无法读取值。");

            object value = pi.GetValue(objectInstance, null);
            return (T)value;
        }

        public static void Set(this object objectInstance, string propertyName, object value)
        {
            Type objectType = objectInstance.GetType();
            PropertyInfo pi = objectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi == null)
                throw new ArgumentException("自动设置参数值失败。参数化变量名称" + propertyName + "必须和对象中的属性名称一样。");
            if (!pi.CanRead)
                throw new ArgumentException("自动设置参数值失败。对象的" + propertyName + "属性没有get方式，无法读取值。");

            pi.SetValue(objectInstance, value);
        }
    }
}
