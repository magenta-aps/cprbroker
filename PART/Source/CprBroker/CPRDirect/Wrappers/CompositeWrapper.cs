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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class CompositeWrapper : Wrapper
    {
        public CompositeWrapper()
        { }

        public CompositeWrapper(int length)
            : base(length)
        { }

        private static string Read(TextReader rd, int count)
        {
            char[] ret = new char[count];
            int read = rd.Read(ret, 0, count);
            if (read != count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return new string(ret);
        }

        public static List<Wrapper> Parse(string data, Dictionary<string, Type> typeMap)
        {
            StringReader rd = new StringReader(data);
            return Parse(rd, typeMap);
        }

        public static List<Wrapper> Parse(TextReader rd, Dictionary<string, Type> typeMap)
        {
            return Parse(rd, typeMap, int.MaxValue);
        }

        public static List<Wrapper> Parse(TextReader rd, Dictionary<string, Type> typeMap, int maxCount)
        {
            var ret = new List<Wrapper>();

            while (rd.Peek() > -1 && ret.Count() < maxCount)
            {
                string typeCode = Read(rd, Constants.DataObjectCodeLength);
                Type type;
                try
                {
                    type = typeMap[typeCode];
                }
                catch { throw; }
                var wrapper = Utilities.Reflection.CreateInstance(type) as Wrapper;
                var subData = Read(rd, wrapper.Length - typeCode.Length);
                wrapper.Contents = typeCode + subData;
                ret.Add(wrapper);

                // Consume new line characters
                while (new int[] { 10, 13 }.Contains((int)rd.Peek()))
                {
                    rd.Read();
                }
            }
            return ret;
        }

        public virtual void FillPropertiesFromWrappers(IList<Wrapper> wrappersIList, params Wrapper[] extraWrappers)
        {
            var wrappers = new List<Wrapper>(wrappersIList);
            wrappers.AddRange(extraWrappers.Where(w => w != null));

            wrappers.RemoveAll(w => w == null);

            Type myType = GetType();
            var properties = myType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                var minMaxAttr = property.GetCustomAttributes(typeof(MinMaxOccurs), true).SingleOrDefault() as MinMaxOccurs;
                if (minMaxAttr == null)
                {
                    continue;
                }
                if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType) && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var args = property.PropertyType.GetGenericArguments();
                    if (args.Length == 1)
                    {
                        var innerType = args[0];
                        if (typeof(Wrapper).IsAssignableFrom(innerType))
                        {
                            var arrayVal = wrappers.Where(obj => obj.GetType() == innerType).ToArray();
                            foreach (var singleVal in arrayVal)
                            {
                                var fieldValue = property.GetValue(this, null);
                                property.PropertyType.InvokeMember(
                                    "Add",
                                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance,
                                    null,
                                    fieldValue,
                                    new object[] { singleVal });
                            }
                            minMaxAttr.ValidateList(arrayVal);
                        }

                    }
                }
                else if (typeof(Wrapper).IsAssignableFrom(property.PropertyType))
                {
                    var val = wrappers.Where(obj => obj.GetType() == property.PropertyType).SingleOrDefault();
                    property.SetValue(this, val, null);
                    minMaxAttr.ValidateSingleObject(val);
                }
            }
        }

        public virtual void FillFromFixedLengthString(string data, Dictionary<string, Type> typeMap)
        {
            var all = Parse(data, typeMap);
            FillPropertiesFromWrappers(all);
        }

        public List<ITimedType> GetChildrenAsTimedObjects()
        {
            return GetChildrenAsType<ITimedType>();
        }

        public List<T> GetChildrenAsType<T>()
            where T : class
        {
            var ret = new List<T>();

            Type myType = GetType();
            var properties = myType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType) && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var args = property.PropertyType.GetGenericArguments();
                    if (args.Length == 1)
                    {
                        var innerType = args[0];
                        if (typeof(T).IsAssignableFrom(innerType))
                        {
                            var fieldValue = property.GetValue(this, null) as System.Collections.IList;
                            foreach (T obj in fieldValue)
                            {
                                ret.Add(obj);
                            }
                        }
                    }
                }
                else if (typeof(T).IsAssignableFrom(property.PropertyType))
                {
                    var val = property.GetValue(this, null) as T;
                    ret.Add(val);
                }
            }
            return ret.Where(o => o != null).ToArray().ToList();
        }
    }
}
