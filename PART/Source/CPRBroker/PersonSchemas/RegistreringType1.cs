using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CprBroker.Schemas.Part
{
    public partial class RegistreringType1
    {
        public void CalculateVirkning()
        {
            this.Virkning = null;
            var partialVirkning = GetPropertyValuesOfType<VirkningType>(this)
                .Where(v => !VirkningType.IsOpen(v))
                .ToArray();
            // TODO: Check this
            this.Virkning = partialVirkning;
        }

        public static T[] GetPropertyValuesOfType<T>(object root) where T : class
        {
            List<T> ret = new List<T>();
            List<object> scannedObjects = new List<object>();
            Type targetType = typeof(T);

            Action<object> method = null;

            method = (subRoot) =>
                {
                    if (subRoot == null || scannedObjects.Contains(subRoot))
                        return;

                    scannedObjects.Add(subRoot);
                    Type subRootType = subRoot.GetType();

                    var props = subRootType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    ret.AddRange(
                        props
                        .Where(p => targetType.IsAssignableFrom(p.PropertyType))
                        .Select(p => p.GetValue(subRoot, null) as T)
                        );

                    var otherProps =
                        props
                        .Where(p => !targetType.IsAssignableFrom(p.PropertyType));

                    foreach (var prop in otherProps)
                    {
                        object value = prop.GetValue(subRoot, null);
                        if (prop.PropertyType.IsArray)
                        {
                            Array arr = value as Array;
                            if (arr != null)
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    method(arr.GetValue(i));
                                }
                            }
                        }
                        else
                        {
                            method(value);
                        }
                    }
                };

            method(root);
            return ret.ToArray();
        }
    }
}
