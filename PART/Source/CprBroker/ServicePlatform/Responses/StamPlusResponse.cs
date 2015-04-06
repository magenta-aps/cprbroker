using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class StamPlusResponse : BaseResponse
    {
        public StamPlusResponse(string xml)
            : base(xml)
        {
        }

        public AdresseType ToAdresseType(CprBroker.Schemas.PersonCivilRegistrationStatusCode code)
        {
            bool greenlandic = false;
            bool foreign = false;

            return new AdresseType()
            {
                Item = greenlandic ? ToGroenlandAdresseType() as AdresseBaseType :
                        foreign ? ToVerdenAdresseType() :
                        ToDanskAdresseType()
            };
        }

        public DanskAdresseType ToDanskAdresseType()
        {
            // TODO: Detect this
            bool empty = false;
            var addressRow = _RowItems.SingleOrDefault();
            if (empty)
            {
                return new DanskAdresseType() { UkendtAdresseIndikator = true };
            }
            else
            {
                return new DanskAdresseType()
                {
                    AddressComplete = new AddressCompleteType()
                    {
                        AddressAccess = new AddressAccessType()
                        {
                            MunicipalityCode = addressRow["KOMKOD"],
                            StreetBuildingIdentifier = addressRow["HUSNR"],
                            StreetCode = addressRow["VEJKOD"]
                        },
                        AddressPostal = new AddressPostalType()
                        {
                            CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, CprBroker.Providers.CprServices.Constants.DenmarkCountryCode.ToString()),
                            StreetName = addressRow["VEJADRNVN"],
                            StreetNameForAddressingName = addressRow["VEJADRNVN"],
                            StreetBuildingIdentifier = addressRow["HUSNR"],
                            FloorIdentifier = addressRow["ETAGE"],
                            SuiteIdentifier = addressRow["SIDEDOER"],
                            DistrictSubdivisionIdentifier = addressRow["BYNVN"],
                            DistrictName = addressRow["POSTNR", "t"],
                            PostCodeIdentifier = addressRow["POSTNR"],
                            // Unsupported
                            MailDeliverySublocationIdentifier = null,
                            PostOfficeBoxIdentifier = null
                        }
                    },

                    SpecielVejkodeIndikator = Schemas.Util.Converters.ToSpecielVejkodeIndikator(addressRow["VEJKOD"]),
                    SpecielVejkodeIndikatorSpecified = true,

                    PostDistriktTekst = addressRow["POSTNR", "t"],
                    UkendtAdresseIndikator = false,

                    // Unsupported
                    AddressPoint = null,
                    NoteTekst = null,

                    // Districts
                    SkoleDistriktTekst = null,
                    SocialDistriktTekst = null,
                    SogneDistriktTekst = null,
                    ValgkredsDistriktTekst = null,
                    PolitiDistriktTekst = null,
                };
            }
        }

        public GroenlandAdresseType ToGroenlandAdresseType()
        {
            // TODO: Detect this
            bool empty = false;
            var addressRow = _RowItems.SingleOrDefault();
            if (empty) 
            {
                return new GroenlandAdresseType() 
                { 
                    UkendtAdresseIndikator = true,
                };
            }
            else 
            {
                return new GroenlandAdresseType()
                {
                    AddressCompleteGreenland = new AddressCompleteGreenlandType()
                    {
                        CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, CprServices.Constants.DenmarkCountryCode.ToString()),
                        MunicipalityCode = addressRow["KOMKOD"],
                        
                        StreetCode = addressRow["VEJKOD"],                        
                        StreetName = addressRow["VEJADRNVN"],
                        StreetNameForAddressingName = addressRow["VEJADRNVN"],
                        
                        StreetBuildingIdentifier = addressRow["HUSNR"],
                        GreenlandBuildingIdentifier = addressRow["BNR"],
                        
                        FloorIdentifier = addressRow["ETAGE"],
                        SuiteIdentifier = addressRow["SIDEDOER"],
                        
                        PostCodeIdentifier = addressRow["POSTNR"],
                        DistrictName = addressRow["POSTNR", "t"],
                        DistrictSubdivisionIdentifier = addressRow["BYNVN"],
                        
                        // Unsupported
                        MailDeliverySublocationIdentifier = null,
                    },
                    SpecielVejkodeIndikator = Schemas.Util.Converters.ToSpecielVejkodeIndikator(addressRow["VEJKOD"]),
                    SpecielVejkodeIndikatorSpecified = default(bool),
                    NoteTekst = default(string),
                    UkendtAdresseIndikator = false
                };
            }
        }

        public DanskAdresseType ToVerdenAdresseType()
        {
            throw new NotImplementedException();
        }


    }
}
