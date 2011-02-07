using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
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
                        CountryIdentificationCode = CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)db.DenmarkAddress.CountrySchemeTypeId, db.DenmarkAddress.CountryCode),
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
