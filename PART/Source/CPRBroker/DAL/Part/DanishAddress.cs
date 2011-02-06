using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class DanishAddress
    {
        public DanskAdresseType ToXmlType()
        {
            return new DanskAdresseType()
            {
                AddressComplete = ToAddressCompleteType(),
                AddressPoint = AddressPoint != null ? AddressPoint.ToXmlType() : null,
                PolitiDistriktTekst = PoliceDistrict,
                PostDistriktTekst = PostDistrict,
                SkoleDistriktTekst = SchoolDistrict,
                SocialDistriktTekst = SocialDistrict,
                SogneDistriktTekst = ParishDistrict,
                SpecielVejkodeIndikator = DenmarkAddress.SpecialRoadCode.HasValue ? DenmarkAddress.SpecialRoadCode.Value : false,
                SpecielVejkodeIndikatorSpecified = DenmarkAddress.SpecialRoadCode.HasValue,
                ValgkredsDistriktTekst = ConstituencyDistrict,
                NoteTekst = DenmarkAddress.Address.Note,
                UkendtAdresseIndikator = DenmarkAddress.Address.IsUnknown,
            };
        }

        public AddressCompleteType ToAddressCompleteType()
        {
            return new AddressCompleteType()
            {
                AddressAccess = new AddressAccessType()
                {
                    MunicipalityCode = DenmarkAddress.MunicipalityCode,
                    StreetBuildingIdentifier = DenmarkAddress.StreetBuildingIdentifier,
                    StreetCode = DenmarkAddress.StreetCode
                },
                AddressPostal = new AddressPostalType()
                {
                    CountryIdentificationCode = null,
                    DistrictName = DenmarkAddress.DistrictName,
                    DistrictSubdivisionIdentifier = DenmarkAddress.DistrictSubdivisionIdentifier,
                    FloorIdentifier = DenmarkAddress.FloorIdentifier,
                    MailDeliverySublocationIdentifier = DenmarkAddress.MailDeliverySublocation,
                    PostCodeIdentifier = DenmarkAddress.PostCodeIdentifier,
                    PostOfficeBoxIdentifier = PostOfficeBoxIdentifier,
                    StreetBuildingIdentifier = DenmarkAddress.StreetBuildingIdentifier,
                    StreetName = DenmarkAddress.StreetName,
                    StreetNameForAddressingName = DenmarkAddress.StreetNameForAddressing
                }
            };
        }

        public static DanishAddress FromXmlType(DanskAdresseType oio)
        {
            return new DanishAddress()
                {
                    DenmarkAddress = new DenmarkAddress()
                    {
                        CountryCode = oio.AddressComplete.AddressPostal.CountryIdentificationCode.Value,
                        CountrySchemeTypeId = (int)oio.AddressComplete.AddressPostal.CountryIdentificationCode.scheme,
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
    }
}
