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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public static class Converters
    {
        public static string DecimalToString(decimal value)
        {
            return value.ToString("G");
        }
        public static string DecimalToString(decimal value, int length)
        {
            int myLength = length;
            if (value < 0)
                myLength--;

            var ret = value.ToString(new string('0', myLength));
            if (ret.Length > length)
            {
                throw new ArgumentOutOfRangeException("value", string.Format("Value <{0}> cannot be fit in <{1}> characters", value, length));
            }
            return ret;
        }

        public static DateTime? ToDateTime(DateTime? value, char uncertainty)
        {
            if (uncertainty == ' ')
                return value;
            else
                return null;
        }

        public static String ToString(string value, char uncertainty)
        {
            if (uncertainty == ' ')
                return value;
            else
                return string.Empty;
        }

        public static string ToPnrStringOrNull(string pnr)
        {
            decimal decimalPnr;
            if (decimal.TryParse(pnr, out decimalPnr))
            {
                if (decimalPnr.ToString().Length == 9 || decimalPnr.ToString().Length == 10)
                {
                    return DecimalToString(decimalPnr, 10);
                }
            }
            return null;
        }

        public static PersonGenderCodeType ToPersonGenderCodeType(char gender)
        {
            switch (gender.ToString().ToUpper()[0])
            {
                case 'M':
                    return PersonGenderCodeType.male;
                    break;
                case 'K':
                    return PersonGenderCodeType.female;
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Invalied value <{0}>, must be either 'M' or 'K'", gender),
                        "gender");
            }
        }

        public static bool ToFolkekirkeMedlemIndikator(char churchRelation)
        {
            switch (churchRelation.ToString().ToUpper()[0])
            {
                case 'F':
                    return true;
                    break;
                case 'M':
                    return true;
                    break;
                case 'S':
                    return false;
                    break;
                case 'A':
                    return false;
                    break;
                case 'U':
                    return false;
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Invalied value <{0}>, must be 'A', 'M', 'F', 'S' or 'D'", churchRelation),
                        "churchRelation");
            }
        }

        public static bool IsValidCorrectionMarker(char correctionMarker)
        {
            return correctionMarker == Constants.CorrectionMarker.OK;
        }

    }
}
