using System;
namespace AttributeBinder.Helpers
{
    public static class ReflectionHelper
    {
        public static object GetPropValue(this object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue(this object src, string propName, object value)
        {
            var pi = src.GetType().GetProperty(propName);
            pi.SetValue(src, value);
        }
    }
}
