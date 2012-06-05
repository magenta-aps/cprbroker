using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class Wrapper
    {
        private string _Contents;
        public string Contents
        {
            get { return _Contents; }
            set
            {
                int len = string.Format("{0}", value).Length;
                if (len != Length)
                {
                    throw new ArgumentOutOfRangeException(
                        "Contents",
                        value,
                        string.Format("Should be exactly {0} characters", Length)
                        );
                }
                _Contents = value;
            }
        }

        public abstract int Length { get; }

        public string this[int pos, int length]
        {
            get
            {
                return Contents.Substring(pos - 1, length);
            }
        }


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
            var ret = new List<Wrapper>();
            StringReader rd = new StringReader(data);
            while (rd.Peek() > 0)
            {
                string typeCode = Read(rd, Constants.DataObjectCodeLength);
                Type type = typeMap[typeCode];
                var wrapper = Utilities.Reflection.CreateInstance(type) as Wrapper;
                var subData = Read(rd, wrapper.Length - typeCode.Length);
                wrapper.Contents = typeCode + subData;
                ret.Add(wrapper);
            }
            return ret;
        }

        public void FillFrom(IList<Wrapper> wrappers)
        {
            Type myType = GetType();
            var fields = myType.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                var minMaxAttr = field.GetCustomAttributes(typeof(MinMaxOccurs), true).SingleOrDefault() as MinMaxOccurs;
                if (minMaxAttr == null)
                {
                    minMaxAttr = new MinMaxOccurs() { MinOccurs = 1, MaxOccurs = 1 };
                }
                if (typeof(System.Collections.IList).IsAssignableFrom(field.FieldType) && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var args = field.FieldType.GetGenericArguments();
                    if (args.Length == 1)
                    {
                        var innerType = args[0];
                        if (typeof(Wrapper).IsAssignableFrom(innerType))
                        {
                            var arrayVal = wrappers.Where(obj => obj.GetType() == innerType).ToArray();
                            foreach (var singleVal in arrayVal)
                            {
                                var fieldValue = field.GetValue(this);
                                field.FieldType.InvokeMember(
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
                else if (typeof(Wrapper).IsAssignableFrom(field.FieldType))
                {
                    var val = wrappers.Where(obj => obj.GetType() == field.FieldType).SingleOrDefault();
                    field.SetValue(this, val);
                    minMaxAttr.ValidateSingleObject(val);
                }
            }
        }

        public void FillFrom(string data, Dictionary<string, Type> typeMap)
        {
            var all = Parse(data, typeMap);
            FillFrom(all);
        }

    }
}
