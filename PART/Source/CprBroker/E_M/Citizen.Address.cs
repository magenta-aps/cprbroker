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
               StreetBuildingIdentifier = this.HouseNumber,
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
            if (Road == null)
            {
                throw new ArgumentException(string.Format("Road property cannot be null"));
            }

            var ret = new CprBroker.Schemas.Part.AddressPostalType()
           {
               CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
               // TODO: See if DistrictSubdivisionIdentifier can be found
               DistrictSubdivisionIdentifier = null,
               // Set floor
               FloorIdentifier = this.Floor,
               // TODO: See if MailDeliverySublocationIdentifier can be found
               MailDeliverySublocationIdentifier = null,
               // Set post code
               PostCodeIdentifier = postCode,
               // Set post district
               DistrictName = postDistrict,
               // TODO: See if PostOfficeBoxIdentifier can be set
               PostOfficeBoxIdentifier = null,
               // Set building identifier
               StreetBuildingIdentifier = this.HouseNumber,
               // Set street name
               StreetName = Road.RoadName,
               // Set street addressing name
               StreetNameForAddressingName = Road.RoadAddressingName,
               // Set suite identifier
               SuiteIdentifier = this.Door,
           };
            return ret;
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
                    var houseRangePostCode = HouseRangePostCodes
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
        }
    }
}
