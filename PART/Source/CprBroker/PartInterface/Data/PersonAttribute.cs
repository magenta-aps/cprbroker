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
using System.Data.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the PersonAttributes table
    /// </summary>
    public partial class PersonAttributes
    {
        public static Schemas.Part.AttributListeType ToXmlType(IQueryable<PersonAttributes> allDb)
        {
            if (allDb != null)
            {
                return new AttributListeType()
                {
                    Egenskab = allDb
                        .Select(db => ToEgenskabType(db))
                        .Where(o => o != null)
                        .ToArray(),

                    RegisterOplysning = allDb
                        .Select(db => ToRegisterOplysningType(db))
                        .Where(o => o != null)
                        .ToArray(),

                    SundhedOplysning = allDb
                        .Select(db => ToSundhedOplysningType(db))
                        .Where(o => o != null)
                        .ToArray(),

                    LokalUdvidelse = null
                };
            }
            return null;
        }

        public static EgenskabType ToEgenskabType(PersonAttributes db)
        {
            if (db != null && db.PersonProperties != null)
            {
                return PersonProperties.ToXmlType(db.PersonProperties);
            }
            return null;
        }

        public static RegisterOplysningType ToRegisterOplysningType(PersonAttributes db)
        {
            if (db != null)
            {
                var ret = new RegisterOplysningType();

                if (db.CprData != null)
                {
                    ret.Item = CprData.ToXmlType(db.CprData);
                }
                else if (db.ForeignCitizenData != null)
                {
                    ret.Item = ForeignCitizenData.ToXmlType(db.ForeignCitizenData);
                }
                else if (db.UnknownCitizenData != null)
                {
                    ret.Item = UnknownCitizenData.ToXmlType(db.UnknownCitizenData);
                }

                if (ret.Item != null)
                {
                    ret.Virkning = Effect.ToVirkningType(db.Effect);
                    return ret;
                }
            }
            return null;
        }

        public static SundhedOplysningType ToSundhedOplysningType(PersonAttributes db)
        {
            if (db != null && db.HealthInformation != null)
            {
                return HealthInformation.ToXmlType(db.HealthInformation);
            }
            return null;
        }

        public static PersonAttributes[] FromXmlType(Schemas.Part.AttributListeType oio)
        {
            var ret = new List<PersonAttributes>();
            if (oio != null)
            {
                ret.AddRange(PersonProperties.FromXmlType(oio.Egenskab));
                ret.AddRange(CprData.FromXmlType(oio.RegisterOplysning));
                ret.AddRange(ForeignCitizenData.FromXmlType(oio.RegisterOplysning));
                ret.AddRange(UnknownCitizenData.FromXmlType(oio.RegisterOplysning));
                ret.AddRange(HealthInformation.FromXmlType(oio.SundhedOplysning));
            }
            return ret.ToArray();
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonAttributes>(pa => pa.Effect);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.PersonProperties);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.CprData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.ForeignCitizenData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.UnknownCitizenData);
            loadOptions.LoadWith<PersonAttributes>(pa => pa.HealthInformation);

            PersonProperties.SetChildLoadOptions(loadOptions);
            CprData.SetChildLoadOptions(loadOptions);
            ForeignCitizenData.SetChildLoadOptions(loadOptions);
        }

    }
}
