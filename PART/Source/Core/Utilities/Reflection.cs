using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CprBroker.Utilities
{
    public static class Reflection
    {
        public static T CreateInstance<T>(string typeName) where T : class
        {
            Type t = Type.GetType(typeName);
            if (t != null)
            {
                object providerObj = t.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null);
                return providerObj as T;
            }
            return null;
        }

        public static object ChangeNamespace(Type targetType, object sourceObject)
        {
            object targetObject;
            Type sourceType = sourceObject.GetType();
            targetObject = targetType.InvokeMember(null, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance, null, null, null);

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            PropertyInfo[] targetProperties = targetType.GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = (from p in targetProperties where p.Name == sourceProperty.Name select p).SingleOrDefault();
                if (targetProperty != null)
                {
                    object sourcePropertyValue = sourceType.InvokeMember(sourceProperty.Name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance, null, sourceObject, null);
                    if (sourcePropertyValue != null)
                    {
                        object targetPropertyValue = null;
                        if (sourceProperty.PropertyType.Namespace == sourceType.Namespace)
                        {
                            targetPropertyValue = ChangeNamespace(targetProperty.PropertyType, sourcePropertyValue);
                        }
                        else
                        {
                            targetPropertyValue = sourcePropertyValue;
                        }
                        try
                        {
                            targetPropertyValue = ConvertValue(targetPropertyValue, targetProperty.PropertyType);
                            targetType.InvokeMember(sourceProperty.Name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance, null, targetObject, new object[] { targetPropertyValue });
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return targetObject;
        }

        private static object ConvertValue(object source, Type targetValueType)
        {
            Type sourceValueType = source.GetType();
            IEnumerable enumerable = source as IEnumerable;
            if (targetValueType.IsArray && enumerable != null)
            {
                return (from object v in enumerable.AsQueryable() select v).ToArray();
            }
            else
            {
                return source;
            }
        }
    }
}
