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
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    public partial class ForeignCitizenData
    {
        /// <summary>
        /// Represents the ForeignCitizenData table
        /// </summary>    
        public static UdenlandskBorgerType ToXmlType(ForeignCitizenData db)
        {
            if (db != null)
            {
                return new UdenlandskBorgerType()
                    {
                        FoedselslandKode = CountryRef.ToXmlType(db.BirthCountryRef),
                        PersonCivilRegistrationReplacementIdentifier = db.CivilRegistrationReplacementIdentifier,
                        PersonIdentifikator = db.PersonIdentifier,
                        SprogKode = ForeignCitizenCountry.ToXmlType(db.ForeignCitizenCountries, false),
                        PersonNationalityCode = ForeignCitizenCountry.ToXmlType(db.ForeignCitizenCountries, true),
                    };
            }
            return null;
        }

        public static ForeignCitizenData FromXmlType(RegisterOplysningType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return FromXmlType(oio[0].Item as UdenlandskBorgerType);
            }
            return null;
        }

        public static ForeignCitizenData FromXmlType(UdenlandskBorgerType oio)
        {
            if (oio != null)
            {
                var ret = new ForeignCitizenData()
                {
                    BirthCountryRef = CountryRef.FromXmlType(oio.FoedselslandKode),
                    CivilRegistrationReplacementIdentifier = oio.PersonCivilRegistrationReplacementIdentifier,
                    PersonIdentifier = oio.PersonIdentifikator,
                };

                ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.SprogKode, false));
                ret.ForeignCitizenCountries.AddRange(ForeignCitizenCountry.FromXmlType(oio.PersonNationalityCode, true));

                return ret;
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<ForeignCitizenData>(fcd => fcd.BirthCountryRef);
            loadOptions.LoadWith<ForeignCitizenData>(fcd => fcd.ForeignCitizenCountries);

            ForeignCitizenCountry.SetChildLoadOptions(loadOptions);
        }
    }
}
