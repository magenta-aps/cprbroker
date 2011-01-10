using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CprBroker.NUnitTester
{
    /// <summary>
    /// Contains utility methods that assist unit testing
    /// </summary>
    public static class Utilities
    {
        public static bool AreEqual<T>(T firstObject, T secondObject)
        {
            return AreEqual<T>("", firstObject, "", secondObject);
        }

        /// <summary>
        /// Determines if two objects are equal by serializing both and comparing the strings
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <param name="firstObject">First object</param>
        /// <param name="secondObject">Second object</param>
        /// <returns></returns>
        public static bool AreEqual<T>(string firstTitle, T firstObject, string secondTitle, T secondObject)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StringWriter writer1 = new System.IO.StringWriter();
            System.IO.StringWriter writer2 = new System.IO.StringWriter();

            serializer.Serialize(writer1, firstObject);
            serializer.Serialize(writer2, secondObject);

            string s1 = writer1.ToString();
            string s2 = writer2.ToString();

            bool ret = s1.Equals(s2);

            try
            {
                if (!ret)
                {
                    string f1 = Path.GetTempFileName();
                    string f2 = Path.GetTempFileName();

                    File.WriteAllText(f1, string.Format("{0}\r\n{1}", firstTitle, s1));
                    File.WriteAllText(f2, string.Format("{0}\r\n{1}", secondTitle, s2));

                    Process p = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {                                                        
                            Arguments = string.Format("\"{0}\" \"{1}\"", f1, f2),
                            //WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory 
                        }
                    };
                    if (File.Exists(@"C:\Program Files (x86)\KDiff3\KDiff3.exe"))
                    {
                        p.StartInfo.FileName = @"C:\Program Files (x86)\KDiff3\KDiff3.exe";
                    }
                    else if (File.Exists(@"C:\Program Files\KDiff3\KDiff3.exe"))
                    {
                        p.StartInfo.FileName = @"C:\Program Files\KDiff3\KDiff3.exe";
                    }
                    p.Start();
                }
            }
            catch
            {
            }
            return ret;
        }

        public static string ArrayToString(string[] data)
        {
            if (data == null)
                return "";
            else
                return string.Join(",", data);
        }


    }
}
