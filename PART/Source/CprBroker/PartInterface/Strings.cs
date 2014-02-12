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

namespace PartInterface
{
    /// <summary>
    /// Contains string processing utility methods
    /// </summary>
    public static class Strings
    {
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
    }
}
