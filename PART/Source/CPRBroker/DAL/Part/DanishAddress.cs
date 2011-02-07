using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
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
