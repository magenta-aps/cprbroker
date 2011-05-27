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
    /// Represents the DanishAddress table
    /// </summary>
    public partial class DanishAddress
    {
        public static DanskAdresseType ToXmlType(DanishAddress db)
        {
            if (db != null && db.DenmarkAddress != null && db.DenmarkAddress.Address != null)
            {
                return new DanskAdresseType()
                {
                    AddressComplete = new AddressCompleteType()
                    {
                        AddressAccess = new AddressAccessType()
                        {
                            MunicipalityCode = db.DenmarkAddress.MunicipalityCode,
                            StreetBuildingIdentifier = db.DenmarkAddress.StreetBuildingIdentifier,
                            StreetCode = db.DenmarkAddress.StreetCode
                        },
                        AddressPostal = new AddressPostalType()
                        {
                            CountryIdentificationCode = null,
                            DistrictName = db.DenmarkAddress.DistrictName,
                            DistrictSubdivisionIdentifier = db.DenmarkAddress.DistrictSubdivisionIdentifier,
                            FloorIdentifier = db.DenmarkAddress.FloorIdentifier,
                            MailDeliverySublocationIdentifier = db.DenmarkAddress.MailDeliverySublocation,
                            PostCodeIdentifier = db.DenmarkAddress.PostCodeIdentifier,
                            PostOfficeBoxIdentifier = db.PostOfficeBoxIdentifier,
                            StreetBuildingIdentifier = db.DenmarkAddress.StreetBuildingIdentifier,
                            StreetName = db.DenmarkAddress.StreetName,
                            StreetNameForAddressingName = db.DenmarkAddress.StreetNameForAddressing
                        }
                    },
                    AddressPoint = AddressPoint.ToXmlType(db.AddressPoint),
                    PolitiDistriktTekst = db.PoliceDistrict,
                    PostDistriktTekst = db.PostDistrict,
                    SkoleDistriktTekst = db.SchoolDistrict,
                    SocialDistriktTekst = db.SocialDistrict,
                    SogneDistriktTekst = db.ParishDistrict,
                    SpecielVejkodeIndikator = db.DenmarkAddress.SpecialRoadCode.HasValue ? db.DenmarkAddress.SpecialRoadCode.Value : false,
                    SpecielVejkodeIndikatorSpecified = db.DenmarkAddress.SpecialRoadCode.HasValue,
                    ValgkredsDistriktTekst = db.ConstituencyDistrict,
                    NoteTekst = db.DenmarkAddress.Address.Note,
                    UkendtAdresseIndikator = db.DenmarkAddress.Address.IsUnknown,
                };
            }
            return null;
        }

        public static DanishAddress FromXmlType(DanskAdresseType oio)
        {
            return new DanishAddress()
                {
                    DenmarkAddress = new DenmarkAddress()
                    {
                        CountryRef = CountryRef.FromXmlType(oio.AddressComplete.AddressPostal.CountryIdentificationCode),
                        DistrictName = oio.AddressComplete.AddressPostal.DistrictName,
                        SpecialRoadCode = oio.SpecielVejkodeIndikator,
                        StreetBuildingIdentifier = oio.AddressComplete.AddressPostal.StreetBuildingIdentifier,
                        StreetCode = oio.AddressComplete.AddressAccess.StreetCode,
                        StreetName = oio.AddressComplete.AddressPostal.StreetName,
                        StreetNameForAddressing = oio.AddressComplete.AddressPostal.StreetNameForAddressingName,
                        DistrictSubdivisionIdentifier = oio.AddressComplete.AddressPostal.DistrictSubdivisionIdentifier,
                        MailDeliverySublocation = oio.AddressComplete.AddressPostal.MailDeliverySublocationIdentifier,
                        MunicipalityCode = oio.AddressComplete.AddressAccess.MunicipalityCode,
                        FloorIdentifier = oio.AddressComplete.AddressPostal.FloorIdentifier,
                        PostCodeIdentifier = oio.AddressComplete.AddressPostal.PostCodeIdentifier,
                        SuiteIdentifier = oio.AddressComplete.AddressPostal.SuiteIdentifier,
                        Address = new Address()
                        {
                            //TODO: Fill
                            IsUnknown = false,
                            Note = null,
                        }
                    },
                    AddressPoint = oio.AddressPoint != null ? AddressPoint.FromXmlType(oio.AddressPoint) : null as AddressPoint,
                    ConstituencyDistrict = oio.ValgkredsDistriktTekst,
                    ParishDistrict = oio.SogneDistriktTekst,
                    PoliceDistrict = oio.PolitiDistriktTekst,
                    PostDistrict = oio.PostDistriktTekst,
                    PostOfficeBoxIdentifier = oio.AddressComplete.AddressPostal.PostOfficeBoxIdentifier,
                    SchoolDistrict = oio.SkoleDistriktTekst,
                    SocialDistrict = oio.SocialDistriktTekst,
                };
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<DanishAddress>(da => da.AddressPoint);

            AddressPoint.SetChildLoadOptions(loadOptions);
        }
    }
}
