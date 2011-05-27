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

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the GreenlandicAddress table
    /// </summary>
    public partial class GreenlandicAddress
    {
        public static GroenlandAdresseType ToXmlType(GreenlandicAddress db)
        {
            if (db != null && db.DenmarkAddress != null && db.DenmarkAddress.Address != null)
            {
                return new GroenlandAdresseType()
                {
                    AddressCompleteGreenland = new AddressCompleteGreenlandType()
                    {
                        CountryIdentificationCode = CountryRef.ToXmlType(db.DenmarkAddress.CountryRef),
                        DistrictName = db.DenmarkAddress.DistrictName,
                        DistrictSubdivisionIdentifier = db.DenmarkAddress.DistrictSubdivisionIdentifier,
                        FloorIdentifier = db.DenmarkAddress.FloorIdentifier,
                        GreenlandBuildingIdentifier = db.GreenlandBuildingIdentifierField,
                        MailDeliverySublocationIdentifier = db.DenmarkAddress.MailDeliverySublocation,
                        MunicipalityCode = db.DenmarkAddress.MunicipalityCode,
                        PostCodeIdentifier = db.DenmarkAddress.PostCodeIdentifier,
                        StreetBuildingIdentifier = db.DenmarkAddress.StreetBuildingIdentifier,
                        StreetCode = db.DenmarkAddress.StreetCode,
                        StreetName = db.DenmarkAddress.StreetName,
                        StreetNameForAddressingName = db.DenmarkAddress.StreetNameForAddressing,
                        SuiteIdentifier = db.DenmarkAddress.SuiteIdentifier,
                    },
                    SpecielVejkodeIndikator = db.DenmarkAddress.SpecialRoadCode.HasValue ? db.DenmarkAddress.SpecialRoadCode.Value : false,
                    SpecielVejkodeIndikatorSpecified = db.DenmarkAddress.SpecialRoadCode.HasValue,
                    NoteTekst = db.DenmarkAddress.Address.Note,
                    UkendtAdresseIndikator = db.DenmarkAddress.Address.IsUnknown,
                };
            }
            return null;
        }

        public static GreenlandicAddress FromXmlType(GroenlandAdresseType oio)
        {
            return new GreenlandicAddress()
            {
                GreenlandBuildingIdentifierField = oio.AddressCompleteGreenland.GreenlandBuildingIdentifier,
                DenmarkAddress = new DenmarkAddress()
                {
                    Address = new Address()
                    {
                        IsUnknown = oio.UkendtAdresseIndikator,
                        Note = oio.NoteTekst,
                    },
                    CountryRef = CountryRef.FromXmlType(oio.AddressCompleteGreenland.CountryIdentificationCode),
                    DistrictName = oio.AddressCompleteGreenland.DistrictName,
                    DistrictSubdivisionIdentifier = oio.AddressCompleteGreenland.DistrictSubdivisionIdentifier,
                    FloorIdentifier = oio.AddressCompleteGreenland.FloorIdentifier,
                    MailDeliverySublocation = oio.AddressCompleteGreenland.MailDeliverySublocationIdentifier,
                    MunicipalityCode = oio.AddressCompleteGreenland.MunicipalityCode,
                    PostCodeIdentifier = oio.AddressCompleteGreenland.PostCodeIdentifier,
                    SpecialRoadCode = oio.SpecielVejkodeIndikatorSpecified ? oio.SpecielVejkodeIndikator : (bool?)null,
                    StreetBuildingIdentifier = oio.AddressCompleteGreenland.StreetBuildingIdentifier,
                    StreetCode = oio.AddressCompleteGreenland.StreetCode,
                    StreetName = oio.AddressCompleteGreenland.StreetName,
                    StreetNameForAddressing = oio.AddressCompleteGreenland.StreetNameForAddressingName,
                    SuiteIdentifier = oio.AddressCompleteGreenland.SuiteIdentifier,
                },
            };
        }
    }
}
