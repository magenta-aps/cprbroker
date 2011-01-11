using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Engine;
using CprBroker.Providers.KMD.WS_AS78207;
using CprBroker.Schemas.Part;
using CprBroker.DAL;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider : IPartReadDataProvider
    {
        public static readonly Guid ActorId = new Guid("{A4E9B3E0-275F-4b76-AADB-4398AA56B871}");

        #region IPartReadDataProvider Members

        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out CprBroker.Schemas.QualityLevel? ql)
        {
            // TODO: Find a solution for effect date in KMD Read
            RegistreringType1 ret = null;
            var resp = new EnglishAS78207Response(CallAS78207(uuid.CprNumber));
            var addressResp = CallAN08002(uuid.CprNumber);
            //var relationsResponse = CallAN08010(uuid.CprNumber);

            bool protectAddress = resp.AddressProtection.Equals("B") || resp.AddressProtection.Equals("L");

            ret = new RegistreringType1()
            {
                AttributListe = new AttributListeType()
                {
                    Egenskaber = new EgenskaberType[]
                    {
                        new EgenskaberType()
                        {
                            PersonBirthDateStructure= new CprBroker.Schemas.Part.PersonBirthDateStructureType()
                            {
                                //TODO: Handle null birthdate
                                BirthDate = ToDateTime(resp.BirthDate).Value,
                                BirthDateUncertaintyIndicator = false
                            },
                            //TODO: Change this
                            PersonGenderCode = ToPartGender(uuid.CprNumber),
                            PersonNameStructure = new PersonNameStructureType(resp.FirstName, resp.LastName),
                            RegisterOplysninger=new RegisterOplysningerType[]
                            {
                                new RegisterOplysningerType()
                                {
                                    //TODO: Fill with CPR, Foreign or Unknown
                                    Item = null,
                                }
                            },
                            Virkning = VirkningType.Create(null,null),
                            /*
                            Attributes = new PersonAttributes()
                            {
                                BirthDate = ToDateTime(resp.BirthDate).Value,
                                ContactChannel = new ContactChannel[0],
                                Gender = ToPartGender(uuid.CprNumber),
                                Name = new Effect<string>()
                                {
                                    StartDate = ToDateTime(resp.NameDate),
                                    EndDate = null,
                                    Value = new PersonNameStructureType(resp.FirstName, resp.LastName).ToString()
                                },
                                // TODO: check if other addresses can be filled from another service (e.g. AN80002)
                                OtherAddresses = new CprBroker.Schemas.Part.Address[0],
                                //TODO: Handle the case of Foreign data or unknown data
                                PersonData = new CprData()
                                {
                                    AddressingName = resp.AddressingName,
                                    BirthDateUncertainty = false,
                                    CprNumber = resp.PNR,
                                    Gender = ToPartGender(uuid.CprNumber),

                                    IndividualTrackStatus = false,
                                    NameAndAddressProtection = protectAddress,
                                    NationalityCountryCode = DAL.Country.GetCountryAlpha2CodeByKmdCode(resp.NationalityCode),
                                    NickName = null,
                                    PersonName = new Effect<PersonNameStructureType>()
                                    {
                                        StartDate = ToDateTime(resp.NameDate),
                                        EndDate = null,
                                        Value = new PersonNameStructureType(resp.FirstName, resp.LastName)
                                    },
                                    PopulationAddress = resp.ToPartAddress(),
                                }
                            }*/                                
                        },
                    }

                },
                // TODO:See if actor text is different from actor ID
                AktoerTekst = KmdDataProvider.ActorId.ToString(),
                // TODO:Fill comment text
                CommentText = null,
                //TODO: Is this the correct status?
                LivscyklusKode = LivscyklusKodeType.Item5,
                RelationListe = new RelationListeType(),
                //TODO: Fill with registration date
                TidspunktDatoTid = TidspunktType.Create(ToDateTime(resp.StatusDate)),
                TilstandListe = new TilstandListeType()
                {
                    //TODO: Fill with orgfaelles:Gyldighed as soon as knowing what that is???
                    //Gyldighed = null,
                    /*
                     CivilStatus = new Effect<CprBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = ToDateTime(resp.MaritalStatusDate),
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToPartMaritalStatus(resp.MaritallStatusCode[0]),
                    },
                    LifeStatus = new Effect<CprBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        //TODO: Status date may not be the correct field (for example, the status may have changed from 01 to  07 at the date, but the life status is still alive)
                        StartDate = ToDateTime(resp.StatusDate),
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToLifeStatus(GetCivilRegistrationStatus(resp.StatusKmd, resp.StatusCpr))
                    },
                     */

                    // No extensions now
                    LokalUdvidelse = new LokalUdvidelseType()
                    {
                        Any = new XmlElement[0]
                    }
                },
                //TODO: Fill
                Virkning = VirkningType.Create(null, null)
            };

            //Children
            if (resp.ChildrenPNRs != null)
            {
                var childPnrs = (from pnr in resp.ChildrenPNRs where pnr.Replace("-", "").Length > 0 select pnr.Replace("-", "")).ToArray();
                var uuids = Array.ConvertAll<string, Guid>(childPnrs, (cpr) => cpr2uuidFunc(cpr));
                ret.RelationListe.Boern = Array.ConvertAll<Guid, PersonFlerRelationType>
                (
                    uuids,
                    (pId) => PersonFlerRelationType.Create(pId, null, null)
                );
            }
            //Father
            if (Convert.ToDecimal(resp.FatherPNR) > 0)
            {
                ret.RelationListe.Fader = new PersonRelationType[] { PersonRelationType.Create(cpr2uuidFunc(resp.FatherPNR), null, null) };
            }
            //Mother
            if (Convert.ToDecimal(resp.MotherPNR) > 0)
            {
                ret.RelationListe.Fader = new PersonRelationType[] { PersonRelationType.Create(cpr2uuidFunc(resp.MotherPNR), null, null) };
            }

            // Spouse
            if (Convert.ToDecimal(resp.SpousePNR) > 0)
            {
                var maritalStatus = Schemas.Util.Enums.ToPartMaritalStatus(resp.MaritallStatusCode[0]);
                var maritalStatusDate = ToDateTime(resp.MaritalStatusDate);
                bool isMarried = maritalStatus == CprBroker.Schemas.Part.Enums.MaritalStatus.married || maritalStatus == CprBroker.Schemas.Part.Enums.MaritalStatus.registeredpartner;
                var spouseUuid = cpr2uuidFunc(resp.SpousePNR);
                ret.RelationListe.Aegtefaelle = new PersonRelationType[]
                {
                    PersonRelationType.Create
                    (
                        spouseUuid,
                        isMarried? maritalStatusDate : null,
                        isMarried? null : maritalStatusDate
                   )
                };                
            }
            ql = CprBroker.Schemas.QualityLevel.Cpr;
            return ret;
        }

        public CprBroker.Schemas.Part.PersonRegistration[] List(CprBroker.Schemas.PersonIdentifier[] uuids, DateTime? effectDate, out CprBroker.Schemas.QualityLevel? ql)
        {
            // TODO: implement List operation on KMD after Read is completed
            throw new NotImplementedException();
        }

        #endregion

        #region Utility methods

        //TODO: Convert to anonymous method withn Search method
        private bool MergeSearchResult(CprBroker.Schemas.PersonIdentifier[] ids, ref CprBroker.Schemas.PersonIdentifier[] finalResult)
        {
            return true;
        }

        private Schemas.Part.PersonGenderCodeType ToPartGender(string cprNumber)
        {
            int cprNum = int.Parse(cprNumber[cprNumber.Length - 1].ToString());
            if (cprNum % 2 == 0)
            {
                return Schemas.Part.PersonGenderCodeType.female;
            }
            else
            {
                return Schemas.Part.PersonGenderCodeType.male;
            }
        }
        //TODO: Remove this method
        private Schemas.Part.Enums.Gender ToOioGender(string cprNumber)
        {
            int cprNum = int.Parse(cprNumber[cprNumber.Length - 1].ToString());
            if (cprNum % 2 == 0)
            {
                return Schemas.Part.Enums.Gender.Female;
            }
            else
            {
                return Schemas.Part.Enums.Gender.Male;
            }
        }
        //TODO: Remove this method
        private char FromPartGender(Schemas.Part.Enums.Gender? gender)
        {
            switch (gender)
            {
                case Schemas.Part.Enums.Gender.Male:
                    return 'M';
                    break;
                case Schemas.Part.Enums.Gender.Female:
                    return 'K';
                    break;
                default:
                    return '*';
                    break;
            }
        }

        private Effect<Schemas.Part.PersonRelation>[] GetPersonRelations(WS_AN08010.ReplyPerson[] persons, CprBroker.Schemas.PersonIdentifier[] personIdentifiers, params string[] typeFilters)
        {
            if (persons == null)
            {
                persons = new WS_AN08010.ReplyPerson[0];
            }

            List<Effect<Schemas.Part.PersonRelation>> ret = new List<Effect<Schemas.Part.PersonRelation>>();

            for (int i = 0; i < persons.Length; i++)
            {
                var person = persons[i];
                if (Array.IndexOf<string>(typeFilters, person.Type) != -1 && person.IsUnknown == false)
                {
                    ret.Add(new Effect<Schemas.Part.PersonRelation>()
                    {
                        StartDate = null,
                        EndDate = null,
                        Value = new CprBroker.Schemas.Part.PersonRelation()
                        {
                            TargetUUID = personIdentifiers[i].UUID.Value,
                        }
                    });
                }
            }
            return ret.ToArray();
        }

        internal static decimal GetCivilRegistrationStatus(string kmdStatus, string cprStatus)
        {
            int iKmd = int.Parse(kmdStatus);
            int iCpr = int.Parse(cprStatus);
            if (iKmd == 1)// >10
            {
                return iCpr * 10;
            }
            else
            {
                // TODO: differentiate betwee 01, 03, 05 & 07 because cprStatus is always 0 here
                return 1;
            }
        }
        #endregion
    }
}
