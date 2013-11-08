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

namespace CprBroker.Providers.E_M
{
    public partial class CitizenReadyAddress
    {
        public virtual AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = this.ToDanskAdresseType()
            };
        }

        public DanskAdresseType ToDanskAdresseType()
        {
            var ret = new DanskAdresseType()
            {
                AddressComplete = this.ToAddressCompleteType(),
                // No address point for persons
                AddressPoint = null,
                // No address note
                NoteTekst = null,
                // No political districts
                PolitiDistriktTekst = null,
                // Set post district
                PostDistriktTekst = this.PostDistrict,
                // No school district
                SkoleDistriktTekst = null,
                // No social disrict
                SocialDistriktTekst = null,
                // No church district
                SogneDistriktTekst = null,
                // Assumed as high road code
                SpecielVejkodeIndikator = ToSpecielVejkodeIndikator(),
                // Always true because SpecielVejkodeIndikator is always set
                SpecielVejkodeIndikatorSpecified = true,
                // Always false
                UkendtAdresseIndikator = false,
                // No election district
                ValgkredsDistriktTekst = null
            };
            return ret;
        }

        public bool ToSpecielVejkodeIndikator()
        {
            if (RoadCode >= 1 && RoadCode <= 9999)
            {
                return this.RoadCode >= 9900;
            }
            else
            {
                throw new ArgumentException(string.Format("RoadCode <{0}> must be between 1 and 9999", RoadCode));
            }
        }

        public AddressCompleteType ToAddressCompleteType()
        {
            return new CprBroker.Schemas.Part.AddressCompleteType()
           {
               AddressAccess = this.ToAddressAccessType(),
               AddressPostal = this.ToAddressPostalType()
           };
        }

        public AddressAccessType ToAddressAccessType()
        {
            return new CprBroker.Schemas.Part.AddressAccessType()
           {
               MunicipalityCode = Converters.ShortToString(this.MunicipalityCode),
               StreetBuildingIdentifier = Converters.ToNeutralString(this.HouseNumber),
               StreetCode = Converters.ShortToString(this.RoadCode)
           };
        }

        /// <summary>
        /// Converts the current object to AddressPostalType object
        /// Post code & district are passed, any values specified in this object (or sub objects) will be ignored.
        /// </summary>
        /// <param name="postCode"></param>
        /// <param name="postDistrict"></param>
        /// <returns></returns>
        public AddressPostalType ToAddressPostalType()
        {
            var ret = new CprBroker.Schemas.Part.AddressPostalType()
           {
               // Set country code
               CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
               
               // Equals city name
               DistrictSubdivisionIdentifier = this.CityName,
               
               // Set floor
               FloorIdentifier = Converters.ToNeutralString(this.Floor),
               
               // MailDeliverySublocationIdentifier is not supported
               MailDeliverySublocationIdentifier = null,
               
               // Set post code
               PostCodeIdentifier = this.PostCode.ToString(),
               
               // Set to post district
               DistrictName = this.PostDistrict,
               
               // PostOfficeBoxIdentifier is not supported
               PostOfficeBoxIdentifier = null,
               
               // Set building identifier
               StreetBuildingIdentifier = Converters.ToNeutralString(this.HouseNumber),
               
               // Set street name
               StreetName = Converters.ToNeutralString(this.RoadName),
               
               // Set street addressing name
               StreetNameForAddressingName = Converters.ToNeutralString(this.GetActiveRoad().RoadAddressingName),
               
               // Set suite identifier
               SuiteIdentifier = Converters.ToNeutralString(this.Door),
           };
            return ret;
        }

        public Road GetActiveRoad()
        {
            if (Roads.Count == 0)
            {
                throw new ArgumentException(string.Format("Road property cannot be null"));
            }
            return this.Roads.OrderByDescending(rd => rd.RoadEndDate).First();
        }
    }
}
