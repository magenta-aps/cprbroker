using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Engine;
using System.Xml;
using CprBroker.Providers.ServicePlatform.Responses;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider
    {
        

        public AttributListeType ToAttributListeType(string stamPlusResponse)
        {
            return new Schemas.Part.AttributListeType()
            {
                Egenskab = new EgenskabType[]
                {
                    new EgenskabType()
                    {
                        NavnStruktur = null,
                        AndreAdresser = null,
                        BirthDate = default(DateTime),
                        FoedestedNavn = null,
                        FoedselsregistreringMyndighedNavn = null,
                        KontaktKanal = null,
                        NaermestePaaroerende = null,
                        PersonGenderCode = default(PersonGenderCodeType),
                        Virkning = null
                    }
                },
                RegisterOplysning = new RegisterOplysningType[] 
                { 
                    new RegisterOplysningType()
                    {
                        Item = new CprBorgerType()
                        {
                            AdresseNoteTekst = null,
                            PersonNummerGyldighedStatusIndikator = default(bool),
                            FolkekirkeMedlemIndikator = default(bool),
                            FolkeregisterAdresse = new AdresseType()
                            {
                                Item = new DanskAdresseType()
                                {
                                    AddressPoint = null,
                                    NoteTekst = null,
                                    SkoleDistriktTekst = null,
                                    SocialDistriktTekst = null,
                                    SpecielVejkodeIndikator = default(bool),
                                    SogneDistriktTekst = null,
                                    SpecielVejkodeIndikatorSpecified = default(bool),
                                    PolitiDistriktTekst = null,
                                    PostDistriktTekst = null,
                                    UkendtAdresseIndikator = default(bool),
                                    ValgkredsDistriktTekst = null,
                                    AddressComplete = new AddressCompleteType()
                                    {
                                        AddressAccess = new AddressAccessType()
                                        {
                                            MunicipalityCode = null,
                                            StreetBuildingIdentifier = null,
                                            StreetCode = null
                                        },
                                        AddressPostal = new AddressPostalType()
                                        {
                                            CountryIdentificationCode = null,
                                            StreetName = null,
                                            StreetNameForAddressingName = null,
                                            StreetBuildingIdentifier = null,
                                            SuiteIdentifier = null,
                                            DistrictSubdivisionIdentifier = null,
                                            MailDeliverySublocationIdentifier = null,
                                            DistrictName = null,
                                            FloorIdentifier = null,
                                            PostCodeIdentifier = null,
                                            PostOfficeBoxIdentifier = null
                                        }
                                    }
                                }
                            },
                            ForskerBeskyttelseIndikator = default(bool),
                            NavneAdresseBeskyttelseIndikator = default(bool),
                            PersonCivilRegistrationIdentifier = null,
                            PersonNationalityCode = null,
                            TelefonNummerBeskyttelseIndikator = default(bool)
                        },
                        Virkning = null,
                    }
                }
            };
        }
    }
}
