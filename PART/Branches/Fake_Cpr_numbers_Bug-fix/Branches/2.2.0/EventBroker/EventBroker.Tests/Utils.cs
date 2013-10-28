using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker.Data;
using System.IO;
using CprBroker.Data.Part;

namespace CprBroker.EventBroker.Tests
{
    public class Utils
    {
        public static RegisterOplysningType[] CreateRegisterOplysning(string municipalityCode)
        {
            return new RegisterOplysningType[]{
                        new RegisterOplysningType() { 
                                    Item = new CprBorgerType() { 
                                        FolkeregisterAdresse = new AdresseType() { 
                                            Item = new DanskAdresseType() { 
                                                AddressComplete = new AddressCompleteType() { 
                                                    AddressAccess = new AddressAccessType() { 
                                                        MunicipalityCode = municipalityCode 
                            } } } } } } };
        }

        public static RegistreringType1 CreateRegistreringType1(string municipalityCode)
        {
            return new RegistreringType1()
            {
                LivscyklusKode = LivscyklusKodeType.Rettet,
                AttributListe = new AttributListeType()
                {
                    RegisterOplysning = Utils.CreateRegisterOplysning(municipalityCode)
                }
            };
        }

        public static Person CreatePerson(Guid uuid)
        {
            return new Person() { UUID = uuid, UserInterfaceKeyText = "" };
        }

        public static PersonRegistration CreatePersonRegistration(Person person, string municipalityCode, Guid personRegistrationId, Guid uuid)
        {
            var personReg = Utils.CreateRegistreringType1(municipalityCode);
            var dbReg = new PersonRegistration()
            {
                Person = person,
                UUID = uuid,
                PersonRegistrationId = personRegistrationId,
                RegistrationDate = DateTime.Now,
                BrokerUpdateDate = DateTime.Now
            };
            dbReg.SetContents(personReg);
            return dbReg;
        }

        public static SoegObjektType CreateSoegObject(string municipalityCode)
        {
            return new SoegObjektType() { SoegAttributListe = new SoegAttributListeType() { SoegRegisterOplysning = CreateRegisterOplysning(municipalityCode) } };
        }

        public static Subscription CreateCriteriaSubscription(string municipalityCode)
        {
            var soegObject = CreateSoegObject(municipalityCode);
            return new Subscription()
            {
                SubscriptionId = Guid.NewGuid(),
                DataSubscription = new DataSubscription(),
                Criteria = System.Xml.Linq.XElement.Load(new StringReader(Utilities.Strings.SerializeObject(soegObject))),
                Created = DateTime.Now,
                Deactivated = null,
                SubscriptionTypeId = (int)CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes.DataChange,
                IsForAllPersons = false,
            };
        }

        public static DataChangeEvent CreateDataChangeEvent(PersonRegistration pr)
        {
            return new DataChangeEvent() { DataChangeEventId = Guid.NewGuid(), DueDate = DateTime.Now, PersonRegistrationId = pr.PersonRegistrationId, PersonUuid = pr.UUID, ReceivedDate = DateTime.Now };
        }
    }
}
