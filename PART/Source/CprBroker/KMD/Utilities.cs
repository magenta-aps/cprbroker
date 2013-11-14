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

namespace CprBroker.Providers.KMD
{
    /// <summary>
    /// Contains utility methods used into KMD
    /// </summary>
    public static class Utilities
    {
        public static DateTime? ToDateTime(string str)
        {
            DateTime ret;
            if (DateTime.TryParseExact(str, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out ret))
            {
                return ret;
            }
            return null;
        }

        public static Schemas.Part.PersonGenderCodeType ToPartGender(string cprNumber)
        {
            int cprNum = int.Parse(cprNumber[cprNumber.Length - 1].ToString());
            if (cprNum % 2 == 0)
            {
                return Schemas.Part.PersonGenderCodeType.female;
            }
            else
            {
                return Schemas.Part.PersonGenderCodeType.male;
            }
        }

        public static decimal GetCivilRegistrationStatus(string kmdStatus, string cprStatus)
        {
            int iKmd = int.Parse(kmdStatus);
            int iCpr = int.Parse(cprStatus);
            if (iKmd == 1)// >10
            {
                return iCpr * 10;
            }
            else
            {
                // TODO: differentiate between 01, 03, 05 & 07 because cprStatus is always 0 here
                return 1;
            }
        }

        public static CivilStatusKodeType ToPartMaritalStatus(char code)
        {
            switch (char.ToUpper(code))
            {
                case 'U':
                    return CivilStatusKodeType.Ugift;
                case 'G':
                    return CivilStatusKodeType.Gift;
                case 'F':
                    return CivilStatusKodeType.Skilt;
                case 'D':
                    // TODO: No deceased status id PART standard
                    return CivilStatusKodeType.Ugift;
                case 'E':
                    return CivilStatusKodeType.Enke;
                case 'P':
                    return CivilStatusKodeType.RegistreretPartner;
                case 'O':
                    return CivilStatusKodeType.OphaevetPartnerskab;
                case 'L':
                default:
                    return CivilStatusKodeType.Laengstlevende;
                // TODO: When to use CivilStatusKode.Separeret?

            }
        }

        public static DateTime? GetMaxDate(params string[] candidateEffectDates)
        {
            return candidateEffectDates
               .Select(s => Utilities.ToDateTime(s))
               .Where(d => d.HasValue)
               .OrderByDescending(d => d.Value)
               .FirstOrDefault();
        }

        public static char FromPartGender(Schemas.Part.PersonGenderCodeType? gender)
        {
            switch (gender)
            {
                case Schemas.Part.PersonGenderCodeType.male:
                    return 'M';
                    break;
                case Schemas.Part.PersonGenderCodeType.female:
                    return 'K';
                    break;
                default:
                    return '*';
                    break;
            }
        }

        public static string GetOperationName(KmdDataProvider.ServiceTypes type)
        {
            return Enum.GetName(typeof(KmdDataProvider.ServiceTypes), type);
        }

    }
}
