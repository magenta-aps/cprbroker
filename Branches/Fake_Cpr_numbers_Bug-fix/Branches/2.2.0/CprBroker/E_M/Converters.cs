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
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public class Converters
    {
        public static bool IsValidCprNumber(decimal cprNumber)
        {
            decimal cprNumber2 = Math.Abs(Math.Floor(cprNumber));
            if (cprNumber2.Equals(cprNumber))
            {
                string s = DecimalToString(cprNumber);
                if (s.Length == 9 || s.Length == 10)
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToCprNumber(decimal cprNumber)
        {
            if (IsValidCprNumber(cprNumber))
            {
                string ret = DecimalToString(cprNumber);
                if (ret.Length == 9)
                {
                    ret = new string('0', 10 - ret.Length) + ret;
                }
                return ret;
            }
            else
            {
                throw new ArgumentException("Invalid CPR number", string.Format("{0}", cprNumber));
            }
        }

        public static CivilStatusKodeType ToCivilStatusKodeType(char status)
        {
            var codes = new CivilStatusCodes();
            if (codes.ContainsKey(status))
            {
                return codes.Map(status);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid input <{0}>", status), "status");
            }
        }

        public static LivStatusKodeType ToLivStatusKodeType(short value, bool birthdateHasValue)
        {
            decimal decimalStatus = (decimal)value;
            return Schemas.Util.Enums.ToLifeStatus(decimalStatus, birthdateHasValue);
        }

        public static DateTime? ToDateTime(DateTime value)
        {
            return ToDateTime(value, ' ');
        }

        public static DateTime? ToDateTime(DateTime value, char uncertainty)
        {
            if (uncertainty == ' '
                && value.Year != DateTime.MinValue.Year
                && value.Year != DateTime.MaxValue.Year
                && value != Constants.MaxEMDate)
            {
                return value;
            }
            return null;
        }

        public static string ShortToString(short val)
        {
            return val.ToString("F0");
        }

        public static string DecimalToString(decimal val)
        {
            return val.ToString("F0");
        }

        public static PersonGenderCodeType ToPersonGenderCodeType(char gender)
        {
            switch (gender.ToString().ToUpper()[0])
            {
                case 'M':
                    return PersonGenderCodeType.male;
                case 'K':
                    return PersonGenderCodeType.female;
            }
            throw new ArgumentException(string.Format("Invalid gender value <{0}>", gender), "gender");
        }

        public static bool ToChurchMembershipIndicator(char churchMarker)
        {
            // F U A M S 
            return churchMarker.ToString().ToUpper() == "F";
        }

        public static DateTime? GetMaxDate(params DateTime?[] dates)
        {
            if (dates != null)
            {
                var datesWithValues = dates.Where(d => d.HasValue).Select(d => d.Value);
                if (datesWithValues.Count() > 0)
                {
                    return datesWithValues.Max();
                }
                else
                {
                    return null;
                }
            }
            throw new ArgumentNullException("dates");
        }

        public static string ToNeutralString(string value)
        {
            return string.Format("{0}", value).Trim();
        }

        public static int ToNeutralHouseNumber(string houseNumber)
        {
            houseNumber = ToNeutralString(houseNumber);
            if (houseNumber.Length > 0)
            {
                string pat = "\\A0*(?<num>[1-9][0-9]*)[a-zA-Z]?\\z";
                var match = System.Text.RegularExpressions.Regex.Match(houseNumber, pat);
                if (match.Success)
                {
                    return int.Parse(match.Groups["num"].Value);
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid format in houseNumber <{0}>", houseNumber));
                }
            }
            else
            {
                return 0;
            }
        }
    }
}