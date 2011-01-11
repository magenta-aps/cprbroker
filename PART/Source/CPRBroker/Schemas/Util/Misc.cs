using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Util
{
    public static class Misc
    {
        public static string FirstNonEmptyString(ref int index, params string[] args)
        {
            for (int i = index; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(args[i]))
                {
                    index = i;
                    return args[i];
                }
            }
            return "";
        }

        public static bool AreEqual<T>(T firstObject, T secondObject)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StringWriter writer1 = new System.IO.StringWriter();
            System.IO.StringWriter writer2 = new System.IO.StringWriter();

            serializer.Serialize(writer1, firstObject);
            serializer.Serialize(writer2, secondObject);

            string s1 = writer1.ToString();
            string s2 = writer2.ToString();

            bool ret = s1.Equals(s2);
            return ret;
        }
    }
}
