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

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider
    {
        public class RelationNode
        {
            XmlElement Node;

            public RelationNode(XmlElement elm)
            {
                Node = elm;
            }

            public string PnrOrBirthdate
            {
                get { return Node.SelectSingleNode("//Field[@r='PNR_FOEDDATO']").Value; }
            }

            public string RelationTypeString
            {
                get { return Node.SelectSingleNode("//Field[@r='FAMMRK']").Value; }
            }

            public PersonFlerRelationType ToPersonFlerRelationType(Func<string, Guid> func)
            {
                return PersonFlerRelationType.Create(func(PnrOrBirthdate), null, null);
            }

            public PersonRelationType ToPersonRelationType(Func<string, Guid> func)
            {
                return PersonRelationType.Create(func(PnrOrBirthdate), null, null);
            }
        }

        public RelationListeType ToRelationListeType(string familyPlusResponse, Func<string, Guid> uuidGetter)
        {
            var doc = new XmlDocument();
            doc.LoadXml(familyPlusResponse);
            var relationNodes = doc.SelectNodes("//Table/Row")
                .OfType<XmlElement>()
                .Select(e => new RelationNode(e))
                .ToArray();

            return new Schemas.Part.RelationListeType()
            {
                Fader = null,
                Moder = null,

                Boern = relationNodes.Where(r => r.RelationTypeString == "Barn").Select(r => r.ToPersonFlerRelationType(uuidGetter)).ToArray(),
                Aegtefaelle = relationNodes.Where(r => r.RelationTypeString == "Ægtefælle").Select(r => r.ToPersonRelationType(uuidGetter)).ToArray(),
                RegistreretPartner = null,

                Bopaelssamling = null,

                ErstatningAf = null,
                ErstatningFor = null,

                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,

                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null,

                LokalUdvidelse = null,
            };
        }

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
