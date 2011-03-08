using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Miscellaneous utility methods
    /// </summary>
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

        public static Exception GetDeepestInnerException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        public static bool ExceptionTreeContainsText(Exception ex,string text)
        {
            if (ex.Message.Contains(text))
            {
                return true;
            }
            else if (ex.InnerException == null)
            {
                return false;
            }
            else
            {
                return ExceptionTreeContainsText(ex.InnerException, text);
            }
        }
    }
}
