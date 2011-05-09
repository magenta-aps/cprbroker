/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Contains some utility functions that assis DPR
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Converts the first not null decimal? to a date
        /// </summary>
        /// <param name="decimalValues">Array of decimals</param>
        /// <returns></returns>
        public static DateTime? DateFromFirstDecimal(params decimal?[] decimalValues)
        {
            DateTime? ret = null;
            foreach (decimal? d in decimalValues)
            {
                ret = DateFromDecimal(d);
                if (ret.HasValue)
                {
                    break;
                }
            }
            return ret;
        }


        public static DateTime? DateFromDecimal(decimal? d)
        {
            if (d.HasValue && d.Value > 0)
            {
                return DateFromDecimalString(d.ToString());
            }
            return null;
        }

        /// <summary>
        /// Converts a decimal in string representation to a date
        /// </summary>
        /// <param name="str">String representing a decimal value</param>
        /// <returns></returns>
        public static DateTime? DateFromDecimalString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                switch (str.Length)
                {
                    case 8:
                        return DateTime.ParseExact(str, "yyyyMMdd", null);

                    case 12:
                        if (str.EndsWith("99"))
                            str = str.Substring(0, 10) + "00";
                        return DateTime.ParseExact(str, "yyyyMMddHHmm", null);

                }
            }
            return null;
        }

        public static decimal? DecimalFromDate(DateTime? date)
        {
            if (date.HasValue)
            {
                if (date.Value.Date < date.Value) // has a time component
                {
                    return Decimal.Parse(date.Value.ToString("yyyyMMddHHmm"));
                }
                else
                {
                    return Decimal.Parse(date.Value.ToString("yyyyMMdd"));
                }
            }
            else
            {
                return null;
            }
        }

        public static DateTime? GetMaxDate(params decimal?[] candidateEffectDates)
        {
            return candidateEffectDates
               .Select(s => Utilities.DateFromDecimal(s))
               .Where(d => d.HasValue)
               .OrderByDescending(d => d.Value)
               .FirstOrDefault();
        }

       

        public static Schemas.Part.PersonGenderCodeType PersonGenderCodeTypeFromChar(char? gen)
        {
            switch (gen)
            {
                case 'M':
                    return Schemas.Part.PersonGenderCodeType.male;
                    break;
                case 'K':
                    return Schemas.Part.PersonGenderCodeType.female;
                    break;
                default:
                    return Schemas.Part.PersonGenderCodeType.unspecified;
                    break;
            }
        }
    
    }

    public static class Extensions
    {
        public static string ToDecimalString(this decimal val)
        {
            return val.ToString("F0");
        }
    }
}
