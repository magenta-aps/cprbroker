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
    public partial class Citizen
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
            string postCode;
            string postDistrict;
            GetPostCodeAndDistrict(out postCode, out postDistrict);

            var ret = new DanskAdresseType()
            {
                AddressComplete = this.ToAddressCompleteType(postCode, postDistrict),
                // No address point for persons
                AddressPoint = null,
                // No address note
                NoteTekst = null,
                // No political districts
                PolitiDistriktTekst = null,
                // Will be set later in this method
                PostDistriktTekst = postDistrict,
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

        public AddressCompleteType ToAddressCompleteType(string postCode, string postDistrict)
        {
            return new CprBroker.Schemas.Part.AddressCompleteType()
           {
               AddressAccess = this.ToAddressAccessType(),
               AddressPostal = this.ToAddressPostalType(postCode, postDistrict)
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
        public AddressPostalType ToAddressPostalType(string postCode, string postDistrict)
        {
            Road activeRoad = GetActiveRoad();

            var ret = new CprBroker.Schemas.Part.AddressPostalType()
           {
               // Set country code
               CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
               // DistrictSubdivisionIdentifier is not supported
               DistrictSubdivisionIdentifier = null,
               // Set floor
               FloorIdentifier = Converters.ToNeutralString(this.Floor),
               // MailDeliverySublocationIdentifier is not supported
               MailDeliverySublocationIdentifier = null,
               // Set post code
               PostCodeIdentifier = postCode,
               // Set post district
               DistrictName = postDistrict,
               // PostOfficeBoxIdentifier is not supported
               PostOfficeBoxIdentifier = null,
               // Set building identifier
               StreetBuildingIdentifier = Converters.ToNeutralString(this.HouseNumber),
               // Set street name
               StreetName = Converters.ToNeutralString(activeRoad.RoadName),
               // Set street addressing name
               StreetNameForAddressingName = Converters.ToNeutralString(activeRoad.RoadAddressingName),
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

        /// <summary>
        /// Tries to extract the post code and postal district name for a citizen, and fill the out parameters
        /// First, looks for a matching record in HousePostCode, by municiplity, road and house numbers
        /// If not found, checks is the municipality and road codes map to a single postal code (and district), and gets them
        /// If not found (or more than one post code is found), tries to match the house with a house number range in HouseRangePostCodes (with the same municipality and road codes and road side)
        /// If found, returns the found post code (this view does not have a post district)
        /// Otherwise returns null for both fields
        /// </summary>
        /// <param name="postCode">The found post code, or null if not found</param>
        /// <param name="postDistrict">The found post district, or null if not found</param>
        public void GetPostCodeAndDistrict(out string postCode, out string postDistrict)
        {
            postCode = null;
            postDistrict = null;

            short postCodeInt = -1;
            if (this.HousePostCode != null)
            {
                postCodeInt = this.HousePostCode.PostCode;
                postDistrict = this.HousePostCode.PostDistrict;
            }
            else // try to find if the whole street belongs to a single Post district
            {
                var roadPostCodes = this.RoadPostCodes.Select(rps => new { rps.PostCode, rps.PostDistrict }).Distinct().ToArray();
                if (roadPostCodes.Length == 1)
                {
                    postCodeInt = roadPostCodes[0].PostCode;
                    postDistrict = roadPostCodes[0].PostDistrict;
                }
                else // Try to see if the house number falls in a specific house range with post code
                {
                    var houseRangePostCodes = this.HouseRangePostCodes.ToArray();
                    var houseNumber = Converters.ToNeutralHouseNumber(this.HouseNumber);
                    var houseRangePostCode = houseRangePostCodes
                        .Where(
                            hrpc => houseNumber >= Converters.ToNeutralHouseNumber(hrpc.FromHouseNumber)
                                && houseNumber <= Converters.ToNeutralHouseNumber(hrpc.ToHouseNumber)
                                ).FirstOrDefault();
                    if (houseRangePostCode != null)
                    {
                        postCodeInt = houseRangePostCode.PostCode;
                        postDistrict = null;
                    }
                }
            }
            if (postCodeInt > -1)
            {
                postCode = Converters.ShortToString(postCodeInt);
            }
            postDistrict = Converters.ToNeutralString(postDistrict);
        }
    }
}
