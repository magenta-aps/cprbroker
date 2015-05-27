using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CprBroker.Schemas.Part
{
    public interface IDanishAddress
    {
        bool UkendtAdresseIndikator { get; }
        bool SpecielVejkodeIndikator { get; }
        bool SpecielVejkodeIndikatorSpecified { get; }
        IDanishCompleteAdderss CompleteAddress { get; }
    }

    public partial class DanskAdresseType : IDanishAddress
    {
        [XmlIgnore]
        IDanishCompleteAdderss IDanishAddress.CompleteAddress
        {
            get { return this.AddressComplete; }
        }
    }

    public partial class GroenlandAdresseType : IDanishAddress
    {
        [XmlIgnore]
        IDanishCompleteAdderss IDanishAddress.CompleteAddress
        {
            get { return this.AddressCompleteGreenland; }
        }
    }

    public interface IDanishCompleteAdderss
    {
        string MunicipalityCode { get; }
        string StreetCode { get; }
        string MailDeliverySublocationIdentifier { get; }
        string StreetName { get; }
        string StreetNameForAddressingName { get; }
        string StreetBuildingIdentifier { get; }
        string SuiteIdentifier { get; }
        string FloorIdentifier { get; }
        string DistrictSubdivisionIdentifier { get; }
        string PostCodeIdentifier { get; }
        string DistrictName { get; }
        CountryIdentificationCodeType CountryIdentificationCode { get; }
    }

    public partial class AddressCompleteType : IDanishCompleteAdderss
    {
        string IDanishCompleteAdderss.MunicipalityCode
        {
            get
            {
                return this.AddressAccess.MunicipalityCode;
            }

        }

        string IDanishCompleteAdderss.StreetCode
        {
            get
            {
                return this.AddressAccess.StreetCode;
            }
        }

        string IDanishCompleteAdderss.MailDeliverySublocationIdentifier
        {
            get
            {
                return this.AddressPostal.MailDeliverySublocationIdentifier;
            }
        }

        string IDanishCompleteAdderss.StreetName
        {
            get
            {
                return this.AddressPostal.StreetName;
            }
        }

        string IDanishCompleteAdderss.StreetNameForAddressingName
        {
            get
            {
                return this.AddressPostal.StreetNameForAddressingName;
            }
        }

        string IDanishCompleteAdderss.StreetBuildingIdentifier
        {
            get
            {
                return this.AddressPostal.StreetBuildingIdentifier;
            }
        }

        string IDanishCompleteAdderss.SuiteIdentifier
        {
            get
            {
                return this.AddressPostal.SuiteIdentifier;
            }
        }

        string IDanishCompleteAdderss.FloorIdentifier
        {
            get
            {
                return this.AddressPostal.FloorIdentifier;
            }

        }

        string IDanishCompleteAdderss.DistrictSubdivisionIdentifier
        {
            get
            {
                return this.AddressPostal.DistrictSubdivisionIdentifier;
            }
        }

        string IDanishCompleteAdderss.PostCodeIdentifier
        {
            get
            {
                return this.AddressPostal.PostCodeIdentifier;
            }
        }

        string IDanishCompleteAdderss.DistrictName
        {
            get
            {
                return this.AddressPostal.DistrictName;
            }
        }

        CountryIdentificationCodeType IDanishCompleteAdderss.CountryIdentificationCode
        {
            get
            {
                return this.AddressPostal.CountryIdentificationCode;
            }
        }
    }
    public partial class AddressCompleteGreenlandType : IDanishCompleteAdderss
    {

    }
}
