using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class Wrapper
    {
        public Wrapper()
        {
            _Contents = new string(' ', Length);
        }

        public Wrapper(int length)
        {
            _Contents = new string(' ', length);
        }

        private string _Contents;
        public string Contents
        {
            get { return _Contents; }
            set
            {
                int len = string.Format("{0}", value).Length;
                if (Length > 0 && len != Length)
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

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }

        private string this[int pos, int length]
        {
            get
            {
                return Contents.Substring(pos - 1, length);
            }
            set
            {
                if (value.Length != length)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("Should be exactly <{0}> characters", length));
                }
                if (this.Length > 0 && pos + length - 1 > this.Length)
                {
                    throw new ArgumentOutOfRangeException("pos,length", string.Format("Should sum to less than or equal to <{0}>", Length));
                }
                int startIndex = pos - 1;
                var charArr =
                    Contents.Take(startIndex)
                    .Concat(value)
                    .Concat(
                        _Contents.Skip(startIndex + length)
                    )
                    .ToArray();
                _Contents = new string(charArr);
            }
        }

        public string GetString(int pos, int len)
        {
            var ret = this[pos, len];
            return ret.Trim();
        }

        public void SetString(string value, int pos, int len)
        {
            value = string.Format("{0}", value);
            if (value.Length < len)
            {
                value = value + new string(' ', len - value.Length);
            }
            this[pos, len] = value;
        }

        public char GetChar(int pos)
        {
            return this[pos, 1][0];
        }

        public void SetChar(char value, int pos)
        {
            this[pos, 1] = value.ToString();
        }

        public decimal GetDecimal(int pos, int length)
        {
            return decimal.Parse(this[pos, length]);
        }

        public void SetDecimal(decimal value, int pos, int length)
        {
            this[pos, length] = Converters.DecimalToString(value, length);
        }

        public DateTime? GetDateTime(int pos, int length, string format)
        {
            DateTime ret;
            if (DateTime.TryParseExact(this[pos, length], format, null, System.Globalization.DateTimeStyles.None, out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }

        public void SetDateTime(DateTime? value, int pos, int length, string format)
        {
            if (value.HasValue)
            {
                this[pos, length] = value.Value.ToString(format);
            }
            else
            {
                this[pos, length] = new string(' ', length);
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
            while (rd.Peek() > -1)
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

        public static List<Wrapper> ParseBatch(string[] dataLines, Dictionary<string, Type> typeMap)
        {
            var ret = dataLines
                .Where(line => line.Length >= Constants.DataObjectCodeLength)
                .Select(dataLine => new LineWrapper(dataLine).ToWrapper(typeMap))
                .Where(w => w != null);

            return ret.ToList();
        }

        public void FillFrom(IList<Wrapper> wrappersIList, params Wrapper[] extraWrappers)
        {
            var wrappers = new List<Wrapper>(wrappersIList);
            wrappers.AddRange(extraWrappers.Where(w => w != null));

            Type myType = GetType();
            var fields = myType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in fields)
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

        public void FillFrom(string data, Dictionary<string, Type> typeMap)
        {
            var all = Parse(data, typeMap);
            FillFrom(all);
        }

    }
}
