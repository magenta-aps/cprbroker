using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Contains string processing utility methods
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Creates a random file name in the given folder path
        /// </summary>
        /// <param name="folder">Path of folder</param>
        /// <param name="extension">File extension</param>
        /// <returns></returns>
        public static string NewUniquePath(string folder, string extension)
        {
            folder = EnsureDirectoryEndSlash(folder);
            return string.Format(
                "{0}{1}.{2}",
                folder,
                Guid.NewGuid(),
                extension
            );
        }

        public static string EnsureDirectoryEndSlash(string directory)
        {
            return EnsureDirectoryEndSlash(directory, true);
        }

        /// <summary>
        /// Ensures that a folder's path ends (or does not end) with a backslash
        /// </summary>
        /// <param name="directory">Directory path</param>
        /// <param name="shouldHaveSlash">Whether to put or remove a slash</param>
        /// <returns></returns>
        public static string EnsureDirectoryEndSlash(string directory, bool shouldHaveSlash)
        {
            bool containsSlash = directory.EndsWith(Path.DirectorySeparatorChar.ToString());
            if (shouldHaveSlash && !containsSlash)
            {
                directory += Path.DirectorySeparatorChar;
            }
            else if (!shouldHaveSlash && containsSlash)
            {
                directory = directory.Substring(0, directory.Length - Path.DirectorySeparatorChar.ToString().Length);
            }
            return directory;
        }

       

        /// <summary>
        /// Converts an object to a string
        /// Returns empty string if object is null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ObjectToString(object o)
        {
            return string.Format("{0}", o);
        }

        public static string RepeatString(string value, int number)
        {
            StringBuilder b = new StringBuilder(value.Length * number);
            for (int i = 0; i < number; i++)
            {
                b.Append(value);
            }
            return b.ToString();
        }

        public static bool IsGuid(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return false;

            string digitPattern = "[a-fA-F0-9]";

            string continuousPattern = digitPattern + "{16}";
            string hyphenatedPattern
                = RepeatString(digitPattern, 8) + "-"
                + RepeatString(digitPattern, 4) + "-"
                + RepeatString(digitPattern, 4) + "-"
                + RepeatString(digitPattern, 4) + "-"
                + RepeatString(digitPattern, 12)
                ;

            string enclosedHyphenatedPattern =
                @"\{"
                + hyphenatedPattern
                + @"\}";
            string enclosedHyphenatedPattern2 =
                @"\("
                + hyphenatedPattern
                + @"\)";

            string prefixPattern = "0[xX]";
            string compositePattern =
                @"\{"
                +prefixPattern+ RepeatString(digitPattern,8) + ", "
                + prefixPattern + RepeatString(digitPattern, 4) + ", "
                + prefixPattern + RepeatString(digitPattern, 4) + ","
                + @"\{"
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + prefixPattern + RepeatString(digitPattern, 2) + ","
                + @"\}"
                + @"\}";

            string guidPattern =
                continuousPattern + "|"
                + hyphenatedPattern + "|"
                + enclosedHyphenatedPattern + "|"
                + enclosedHyphenatedPattern2 + "|"
                + compositePattern
                ;

            Regex guidReg = new Regex(guidPattern);
            return guidReg.IsMatch(stringValue);
        }

        public static DateTime? PersonNumberToDate(string cprNumber)
        {
            int day;
            int month;
            int year;
            int serialNo;
            try
            {
                if (!string.IsNullOrEmpty(cprNumber)
                    && cprNumber.Length >= 7
                    && int.TryParse(cprNumber.Substring(0, 2), out day)
                    && int.TryParse(cprNumber.Substring(2, 2), out month)
                    && int.TryParse(cprNumber.Substring(4, 2), out year)
                    && int.TryParse(cprNumber.Substring(6, 1), out serialNo)
                    )
                {
                    int centuryYears = 1900;
                    if (
                        (serialNo == 4 && year <= 36)
                        || (serialNo >= 5 && serialNo <= 8 && year <= 57)
                        || (serialNo == 9 && year <= 36)
                        )
                    {
                        centuryYears = 2000;
                    }
                    return new DateTime(centuryYears + year, month, day);
                }
            }
            catch { }
            return null;
        }

        public static Uri GuidToUri(Guid uuid)
        {
            return new Uri(string.Format("urn:uuid:{0}", uuid.ToString("")));
        }

        /// <summary>
        /// Serializes the given object to an XML string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            string ret = "";
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, obj);
                ret = writer.ToString();
            }
            return ret;
        }

        public static T Deserialize<T>(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader writer = new StringReader(xml);
                object o = serializer.Deserialize(writer);
                return (T)o;
            }
            return default(T);
        }

    }

}
