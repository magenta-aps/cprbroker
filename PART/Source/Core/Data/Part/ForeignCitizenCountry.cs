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
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the ForeignCitizenCountry table
    /// Acts as a nationality or as a language (based on IsNationality column)
    /// </summary>
    public partial class ForeignCitizenCountry
    {
        public static CountryIdentificationCodeType ToXmlType(ForeignCitizenCountry db)
        {
            if (db != null)
            {
                return CountryRef.ToXmlType(db.CountryRef);
            }
            return null;
        }

        public static CountryIdentificationCodeType[] ToXmlType(EntitySet<ForeignCitizenCountry> fcc, bool isNationality)
        {
            if (fcc != null)
            {
                return fcc.Where(f => f.IsNationality = isNationality)
                    .OrderBy(f => f.Ordinal)
                    .Select(f => ForeignCitizenCountry.ToXmlType(f))
                    .Where(c => c != null)
                    .ToArray();
            }
            return null;
        }

        public static ForeignCitizenCountry FromXmlType(CountryIdentificationCodeType oio, bool isNationality, int ordinal)
        {
            if (oio != null)
            {
                return new ForeignCitizenCountry()
                {
                    ForeignCitizenCountryId = Guid.NewGuid(),
                    CountryRef = CountryRef.FromXmlType(oio),
                    IsNationality = isNationality,
                    Ordinal = ordinal,
                };
            }
            return null;
        }

        public static ForeignCitizenCountry[] FromXmlType(CountryIdentificationCodeType[] countries, bool isNationality)
        {
            if (countries != null)
            {
                int ordinal = 0;
                return countries
                    .Select(c => ForeignCitizenCountry.FromXmlType(c, isNationality, ordinal++))
                    .Where(c => c != null)
                    .ToArray();
            }
            return new ForeignCitizenCountry[0];
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<ForeignCitizenCountry>(fcc => fcc.CountryRef);
        }
    }

}
