using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class GreenlandicAddress
    {
        public GroenlandAdresseType ToXmlType()
        {
            return new GroenlandAdresseType()
            {
                AddressCompleteGreenland = new AddressCompleteGreenlandType()
                {
                    CountryIdentificationCode = CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)DenmarkAddress.CountrySchemeTypeId, DenmarkAddress.CountryCode),
                    DistrictName = DenmarkAddress.DistrictName,
                    DistrictSubdivisionIdentifier = DenmarkAddress.DistrictSubdivisionIdentifier,
                    FloorIdentifier = DenmarkAddress.FloorIdentifier,
                    GreenlandBuildingIdentifier = this.GreenlandBuildingIdentifierField,
                    MailDeliverySublocationIdentifier = DenmarkAddress.MailDeliverySublocation,
                    MunicipalityCode = DenmarkAddress.MunicipalityCode,
                    PostCodeIdentifier = DenmarkAddress.PostCodeIdentifier,
                    StreetBuildingIdentifier = DenmarkAddress.StreetBuildingIdentifier,
                    StreetCode = DenmarkAddress.StreetCode,
                    StreetName = DenmarkAddress.StreetName,
                    StreetNameForAddressingName = DenmarkAddress.StreetNameForAddressing,
                    SuiteIdentifier = DenmarkAddress.SuiteIdentifier,
                },
                SpecielVejkodeIndikator = DenmarkAddress.SpecialRoadCode.HasValue ? DenmarkAddress.SpecialRoadCode.Value : false,
                SpecielVejkodeIndikatorSpecified = DenmarkAddress.SpecialRoadCode.HasValue,
                NoteTekst = DenmarkAddress.Address.Note,
                UkendtAdresseIndikator = DenmarkAddress.Address.IsUnknown,
            };
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
                    CountryCode = oio.AddressCompleteGreenland.CountryIdentificationCode.Value,
                    CountrySchemeTypeId = (int)oio.AddressCompleteGreenland.CountryIdentificationCode.scheme,
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
