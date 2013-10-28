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
    public partial class HistoricalAddressType : IAddressSource, IHasCorrectionMarker
    {
        public DataTypeTags Tag
        {
            get { return DataTypeTags.Address; }
        }

        public DateTime? ToStartTS()
        {
            return this.RelocationDate;
            //return Converters.ToDateTime(this.RelocationDate, this.RelocationDateUncertainty);
        }

        public DateTime? ToEndTS()
        {
            return this.LeavingDate;
            //return Converters.ToDateTime(this.LeavingDate, this.LeavingDateUncertainty);
        }


        public AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = ToDanskAdresseType()
            };
        }

        public DanskAdresseType ToDanskAdresseType()
        {
            var ret = new DanskAdresseType()
            {
                AddressComplete = this.ToAddressCompleteType(),

                // No address point for persons
                AddressPoint = this.ToAddressPointType(),

                NoteTekst = ToAddressNoteTekste(),

                // No political districts
                PolitiDistriktTekst = null,

                // TODO: Lookup post district
                PostDistriktTekst = null,

                // No school district
                SkoleDistriktTekst = null,

                // No social disrict
                SocialDistriktTekst = null,

                // No church district - checked
                SogneDistriktTekst = null,

                // Assuming this is the same as high road code - verified
                SpecielVejkodeIndikator = this.ToSpecielVejkodeIndikator(),

                // Always true because SpecielVejkodeIndikator is always set                
                SpecielVejkodeIndikatorSpecified = true,

                // Address is unknown if it is empty :)
                // TODO: Make sure that a historical address is never empty
                UkendtAdresseIndikator = false,

                // No election district - checked
                ValgkredsDistriktTekst = null
            };
            return ret;
        }

        public AddressCompleteType ToAddressCompleteType()
        {
            return new CprBroker.Schemas.Part.AddressCompleteType()
            {
                AddressAccess = this.ToAddressAccessType(),
                AddressPostal = this.ToAddressPostalType()
            };
        }

        public string ToAddressNoteTekste()
        {
            return null;
        }

        public AddressPointType ToAddressPointType()
        {
            // Not implemented
            return null;
        }

        public bool ToSpecielVejkodeIndikator()
        {
            return Schemas.Util.Converters.ToSpecielVejkodeIndikator(this.StreetCode);
        }

        public AddressAccessType ToAddressAccessType()
        {
            return new CprBroker.Schemas.Part.AddressAccessType()
            {
                MunicipalityCode = Converters.DecimalToString(this.MunicipalityCode),
                StreetBuildingIdentifier = this.HouseNumber,
                StreetCode = Converters.DecimalToString(this.StreetCode)
            };
        }

        /// <summary>
        /// Converts the current object to AddressPostalType object
        /// </summary>
        /// <returns></returns>
        public AddressPostalType ToAddressPostalType()
        {
            var ret = new CprBroker.Schemas.Part.AddressPostalType()
            {
                // Set country code
                CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),

                // city name
                // TODO: Lookup historical city name
                DistrictSubdivisionIdentifier = null,

                // Set floor
                FloorIdentifier = this.Floor,

                // MailDeliverySublocationIdentifier is not supported - checked
                MailDeliverySublocationIdentifier = null,

                // Set post code
                // TODO: Get historical post code
                PostCodeIdentifier = null,

                // Set district name
                // TODO: Lookup historical DistrictName
                DistrictName = null,

                // PostOfficeBoxIdentifier is not supported
                PostOfficeBoxIdentifier = null,

                // Set building identifier
                StreetBuildingIdentifier = this.HouseNumber,

                // Set street name
                // TODO: Get Street name by looking up street code
                StreetName = null,

                // Set street addressing name
                // TODO: Get Street addressing name by looking up street code
                StreetNameForAddressingName = null,

                // Set suite identifier
                // TODO: Sometimes the suite is '  th'. Shall we set this as empty string in this case?
                SuiteIdentifier = this.Door,
            };
            return ret;
        }

    }
}
