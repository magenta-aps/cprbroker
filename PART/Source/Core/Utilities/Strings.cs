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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
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

        private static readonly Random Random = new Random();

        public static string NewRandomString(int length)
        {
            var ret = "";
            for (int i = 0; i < length; i++)
            {
                ret += Random.Next(0, 10).ToString();
            }
            return ret;
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
            return EnsureEndString(directory, Path.DirectorySeparatorChar.ToString(), shouldHaveSlash);
        }

        public static string EnsureEndString(string str, string endString, bool shouldHaveSlash)
        {
            return EnsureEndString(str, endString, shouldHaveSlash, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Ensures that a folder's path ends (or does not end) with a backslash
        /// </summary>
        /// <param name="str">String to check</param>
        /// <param name="endString">The character that should/should not be at the end</param>
        /// <param name="shouldHaveSlash">Whether to put or remove a slash</param>
        /// <returns></returns>
        public static string EnsureEndString(string str, string endString, bool shouldHaveSlash, StringComparison comparison)
        {
            bool containsEndChar = str.EndsWith(endString.ToString(), comparison);
            if (shouldHaveSlash && !containsEndChar)
            {
                str += endString;
            }
            else if (!shouldHaveSlash && containsEndChar)
            {
                str = str.Substring(0, str.Length - endString.Length);
            }
            return str;
        }

        public static string EnsureStartString(string str, string startString, bool shouldHaveChar)
        {
            return EnsureStartString(str, startString, shouldHaveChar, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Ensures that a folder's path ends (or does not end) with a backslash
        /// </summary>
        /// <param name="str">String to check</param>
        /// <param name="startString">The character that should/should not be at the end</param>
        /// <param name="shouldHaveChar">Whether to put or remove a slash</param>
        /// <returns></returns>
        public static string EnsureStartString(string str, string startString, bool shouldHaveChar, StringComparison comparison)
        {
            bool containsStartChar = str.StartsWith(startString, comparison);
            if (shouldHaveChar && !containsStartChar)
            {
                str = startString + str;
            }
            else if (!shouldHaveChar && containsStartChar)
            {
                str = str.Substring(startString.Length);
            }
            return str;
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
                + prefixPattern + RepeatString(digitPattern, 8) + ", "
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

        public static bool IsValidPersonNumber(string cprNumber)
        {
            if (cprNumber == null)
            {
                return false;
            }
            var pattern = @"\A\d{10}\Z";
            if (!System.Text.RegularExpressions.Regex.Match(cprNumber, pattern).Success)
            {
                return false;
            }

            long val;
            if (!long.TryParse(cprNumber, out val))
            {
                return false;
            }

            if (!Strings.PersonNumberToDate(cprNumber).HasValue)
            {
                return false;
            }

            if (!Strings.IsModulus11OK(cprNumber))
            {
                return false;
            }
            return true;
        }

        public static bool IsModulus11OK(string cprNumber)
        {
            bool result = false;
            int[] multiplyBy = { 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };
            int Sum = 0;
            // We test if the length of the CPR number is right and if the number does not conatain tailing 0's
            if (cprNumber.Substring(6, 4) != "0000")
            {
                /*
                 * We cannot do modulus control on people with birth dates 19650101 or 19660101,
                 * thus those dates just pass through with no control at all.
                 */
                if (cprNumber.Substring(0, 6) == "010165" || cprNumber.Substring(0, 6) == "010166")
                {
                    result = true;
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Sum += Convert.ToInt32(cprNumber.Substring(i, 1)) * multiplyBy[i];
                    }
                    if ((Sum % 11) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
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


        public static string SerializeObject(object obj)
        {
            return SerializeObject(obj, false);
        }
        /// <summary>
        /// Serializes the given object to an XML string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj, bool excludeDeclaration)
        {
            string ret = "";
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, obj);
                ret = writer.ToString();

                if (excludeDeclaration)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(ret);
                    ret = doc.ChildNodes[1].OuterXml;
                }
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

        public static bool IsValidName(string name)
        {
            string pat = @"\A\w[\w\d_]*\Z";
            if (!System.Text.RegularExpressions.Regex.Match(name, pat).Success)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidHostName(string name)
        {
            if (!Regex.Match(name, @"\A[0-9A-Za-z\-\.]+\z").Success)
                return false;

            string[] labels = name.Split('.');
            // eliminate empty host
            if (labels.Length == 0)
                return false;

            // eliminate empty labels
            if (labels.Where(l => string.IsNullOrEmpty(l)).Count() > 0)
                return false;

            // eliminate hosts that look like IP addresses 
            if (labels.Where(l => Regex.Match(l, @"\A[0-9]*\z").Success).Count() == labels.Length)
                return false;

            foreach (var label in labels)
            {
                // label length
                if (label.Length == 0 || label.Length > 63)
                    return false;

                // eliminate hyphen start/end
                if (label.StartsWith("-") || label.EndsWith("-"))
                    return false;

                //// contain at least one alpha character
                //if (!Regex.Match(label, "[a-zA-Z]+").Success)
                //    return false;

                var labelParts = label.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var labelPart in labelParts)
                {
                    // eliminate consecutive hyphens 
                    if (labelPart.Length == 0)
                        return false;
                }
            }
            return true;
        }

    }

}
