using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Providers.E_M
{
    public partial class CitizenPotReadyAddress
    {
        internal static AdresseType ToAdresseType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new AdresseType()
                {
                    Item = ToDanskAdresseType(address)
                };
            }
            return null;
        }

        internal static DanskAdresseType ToDanskAdresseType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new DanskAdresseType()
                {
                    AddressComplete = ToAddressCompleteType(address),
                    // No address point
                    AddressPoint = null,
                    NoteTekst = null,
                    PolitiDistriktTekst = null,
                    PostDistriktTekst = null,
                    SkoleDistriktTekst = null,
                    SocialDistriktTekst = null,
                    SogneDistriktTekst = null,
                    SpecielVejkodeIndikator = false,
                    SpecielVejkodeIndikatorSpecified = false,
                    UkendtAdresseIndikator = false,
                    ValgkredsDistriktTekst = null
                };
            }
            return null;
        }

        internal static AddressCompleteType ToAddressCompleteType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressCompleteType()
               {
                   AddressAccess = ToAddressAccessType(address),
                   AddressPostal = ToAddressPostalType(address)
               };
            }
            return null;
        }

        //TODO: What is Building number(Citizen table) vs house number
        internal static AddressAccessType ToAddressAccessType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressAccessType()
               {
                   MunicipalityCode = Converters.ShortToString(address.MunicipalityCode),
                   StreetBuildingIdentifier = address.HouseNumber,
                   StreetCode = Converters.ShortToString(address.RoadCode)
               };
            }
            return null;
        }

        internal static AddressPostalType ToAddressPostalType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressPostalType()
               {
                   CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                   DistrictName = address.PostDistrict,
                   DistrictSubdivisionIdentifier = null,
                   FloorIdentifier = address.Floor,
                   MailDeliverySublocationIdentifier = null,
                   PostCodeIdentifier = Converters.ShortToString(address.PostCode),
                   PostOfficeBoxIdentifier = null,
                   StreetBuildingIdentifier = address.HouseNumber,
                   StreetName = address.RoadName,
                   StreetNameForAddressingName = null,
                   SuiteIdentifier = address.Door,
               };
            }
            return null;
        }

        [TestFixture]
        public class Tests
        {
            CitizenPotReadyAddress[] AddressTestValues = new CitizenPotReadyAddress[] 
            {
                null,
                new CitizenPotReadyAddress()
            };

            void ValidateNulls<T>(CitizenPotReadyAddress address, T result) where T : class
            {
                if (address == null)
                {
                    Assert.IsNull(result);
                }
                else
                {
                    Assert.IsNotNull(result);
                }
            }

            
            [TestCaseSource("AddressTestValues")]
            public void TestAdresseType(CitizenPotReadyAddress address)
            {
                var result = ToAdresseType(address);
                ValidateNulls<AdresseType>(address, result);
            }

            [TestCaseSource("AddressTestValues")]
            public void TestDanskAdresseType(CitizenPotReadyAddress address)
            {
                var result = ToDanskAdresseType(address);
                ValidateNulls<DanskAdresseType>(address, result);
            }

            [TestCaseSource("AddressTestValues")]
            public void TestToAddressCompleteType(CitizenPotReadyAddress address)
            {
                var result = ToAddressCompleteType(address);
                ValidateNulls<AddressCompleteType>(address, result);
            }

            [TestCaseSource("AddressTestValues")]
            public void TestToAddressAccessType(CitizenPotReadyAddress address)
            {
                var result = ToAddressAccessType(address);
                ValidateNulls<AddressAccessType>(address, result);
            }

            [TestCaseSource("AddressTestValues")]
            public void TestToAddressPostalType(CitizenPotReadyAddress address)
            {
                var result = ToAddressPostalType(address);
                ValidateNulls<AddressPostalType>(address, result);
            }
        }
    }
}
