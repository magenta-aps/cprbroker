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

namespace CprBroker.Schemas.Util
{
    /// <summary>
    /// Contains some utility methods related to OIO enumerations
    /// </summary>
    public static class Enums
    {

        public static LivStatusKodeType ToLifeStatus(decimal civilRegistrationStatus, DateTime? birthDate)
        {
            return ToLifeStatus(civilRegistrationStatus, birthDate.HasValue);
        }

        public static LivStatusKodeType ToLifeStatus(decimal civilRegistrationStatus, bool birthdateHasValue)
        {
            if (!IsValidCivilRegistrationStatus(civilRegistrationStatus))
            {
                throw new ArgumentException(string.Format("Invalid value <{0}> for civilRegistrationStatus", civilRegistrationStatus));
            }
            switch ((int)civilRegistrationStatus)
            {
                case 70:
                    return LivStatusKodeType.Forsvundet;
                case 90:
                    return LivStatusKodeType.Doed;
                default:
                    if (birthdateHasValue)
                        return LivStatusKodeType.Foedt;
                    else
                        return LivStatusKodeType.Prenatal;
            }
        }

        public static KeyValuePair<string, TEnum>[] GetEnumValues<TEnum>() where TEnum : struct
        {
            Type t = typeof(TEnum);
            var names = Enum.GetNames(t);

            Func<string, string> valueGetter =
                (name) =>
                {
                    TEnum enumValue = (TEnum)Enum.Parse(t, name);
                    string ret = ((int)(object)enumValue).ToString();
                    System.Reflection.FieldInfo fieldInfo = t.GetField(name);
                    System.Xml.Serialization.XmlEnumAttribute attr = fieldInfo.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false).SingleOrDefault() as System.Xml.Serialization.XmlEnumAttribute;
                    if (attr != null)
                    {
                        ret = attr.Name;
                    }
                    return ret;
                };
            return (from name in names select new KeyValuePair<string, TEnum>(valueGetter(name), (TEnum)Enum.Parse(t, name))).ToArray();

        }

        /// <summary>
        /// Checks whether the specified civil registration status is valid.
        /// Works according to http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/11/24/cpr_personcivilregistrationstatuscode.xsd.meta.xml
        /// </summary>
        /// <param name="civilRegistrationStatus"></param>
        /// <returns>True if the status code is valid, false otherwise</returns>
        public static bool IsValidCivilRegistrationStatus(decimal civilRegistrationStatus)
        {
            var validStates = GetEnumValues<PersonCivilRegistrationStatusCode>().Select(s => (decimal)s.Value);
            return validStates.Contains(civilRegistrationStatus);
        }

        /// <summary>
        /// Tells whether the specified civil registration status refers to an active CPR number
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown if the value in<paramref name="civilRegistrationStatus"/> is an invalid status </exception>
        /// <param name="civilRegistrationStatus"></param>
        /// <returns>True if the status means an active CPR number, false otherwise</returns>
        public static bool IsActiveCivilRegistrationStatus(decimal civilRegistrationStatus)
        {
            if (!IsValidCivilRegistrationStatus(civilRegistrationStatus))
            {
                throw new ArgumentException(string.Format("Invalid value <{0}> for civilRegistrationStatus", civilRegistrationStatus));
            }
            var inactiveStates = new decimal[] { 
                (decimal)PersonCivilRegistrationStatusCode.CancelledCivilRegistrationNumbers, 
                (decimal)PersonCivilRegistrationStatusCode.ChangedCivilRegistrationNumbers, 
                (decimal)PersonCivilRegistrationStatusCode.DeletedCivilRegistrationNumbers };
            return !inactiveStates.Contains(civilRegistrationStatus);
        }

        public static PersonGenderCodeType PersonNumberToGender(string cprNumber)
        {
            return (long.Parse(cprNumber) % 2 == 1) ? PersonGenderCodeType.male : PersonGenderCodeType.female;
        }

        
    }
}
