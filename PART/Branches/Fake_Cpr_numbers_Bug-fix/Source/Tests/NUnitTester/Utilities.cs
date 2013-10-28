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

        public static readonly Random Random = new Random();
        public static string RandomCprNumber()
        {
            var day = Random.Next(1, 29).ToString("00");
            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9999).ToString();
            return day + month + year + part1;
        }

        public static string[] RandomCprNumbers(int count)
        {
            var ret = new string[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = RandomCprNumber();
            }
            return ret;
        }




    }
}
