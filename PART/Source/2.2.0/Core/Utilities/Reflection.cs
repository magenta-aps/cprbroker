/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Utility methods related to reflection
    /// </summary>
    public static class Reflection
    {
        public static T CreateInstance<T>(string typeName) where T : class
        {
            Type t = Type.GetType(typeName);
            return CreateInstance<T>(t);
        }

        public static T CreateInstance<T>(Type t) where T : class
        {
            if (t != null)
            {
                object ret = t.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null);
                return ret as T;
            }
            return null;
        }

        public static object CreateInstance(Type type)
        {
            return type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null);
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

        public static string CurrentExePath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (System.Web.HttpContext.Current != null)
            {
                path += "bin\\";
            }
            return path;
        }
    }
}
