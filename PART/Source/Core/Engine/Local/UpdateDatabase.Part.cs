using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.DAL;
using CprBroker.DAL.Events;
using CprBroker.DAL.Part;

namespace CprBroker.Engine.Local
{

    public partial class UpdateDatabase
    {
        /// <summary>
        /// Updates the system database with person registration objects
        /// </summary>
        /// <param name="personIdentifier"></param>
        /// <param name="personRegistraion"></param>
        public static void UpdatePersonRegistration(PersonIdentifier personIdentifier, Schemas.Part.RegistreringType1 personRegistraion)
        {
            if (MergePersonRegistration(personIdentifier, personRegistraion))
            {
                // TODO: move this call to a separate phase in request processing
                NotifyPersonRegistrationUpdate(personIdentifier.UUID.Value);
            }
        }

        private static void NotifyPersonRegistrationUpdate(Guid personUuid)
        {
            using (var dataContext = new DataChangeEventDataContext())
            {
                var pp = new DataChangeEvent()
                {
                    DataChangeEventId = Guid.NewGuid(),
                    PersonUuid = personUuid,
                    ReceivedDate = DateTime.Now
                };
                dataContext.DataChangeEvents.InsertOnSubmit(pp);
                dataContext.SubmitChanges();
            }
        }

        private static bool MergePersonRegistration(PersonIdentifier personIdentifier, Schemas.Part.RegistreringType1 oioRegistration)
        {
            //TODO: Modify this method to allow searching for registrations that have a fake date of Today, these should be matched by content rather than registration date
            using (var dataContext = new PartDataContext())
            {
                // Match db registrations by UUID, ActorId and registration date
                var existingInDb = (
                        from dbReg in dataContext.PersonRegistrations
                        where dbReg.UUID == personIdentifier.UUID
                        && dbReg.RegistrationDate == TidspunktType.ToDateTime(oioRegistration.Tidspunkt)
                        &&
                        (
                            (
                                oioRegistration.AktoerRef == null
                                && dbReg.ActorRef == null
                            )
                            ||
                            (
                                oioRegistration.AktoerRef != null
                                && dbReg.ActorRef != null
                                && oioRegistration.AktoerRef.Item == dbReg.ActorRef.Value
                                && (int)oioRegistration.AktoerRef.ItemElementName == dbReg.ActorRef.Type
                            )
                        )
                        select dbReg
                    ).ToArray();

                var duplicateExists = existingInDb.Length > 0;

                // Perform a content match if key match is found
                if (duplicateExists)
                {
                    duplicateExists = Array.Exists<PersonRegistration>(existingInDb, db => db.Equals(oioRegistration));
                }

                // If there are really no matches, update the database
                if (!duplicateExists)
                {
                    var dbPerson = (from dbPers in dataContext.Persons
                                    where dbPers.UUID == personIdentifier.UUID
                                    select dbPers).FirstOrDefault();
                    if (dbPerson == null)
                    {
                        dbPerson = new CprBroker.DAL.Part.Person()
                        {
                            UUID = personIdentifier.UUID.Value,
                            UserInterfaceKeyText = personIdentifier.CprNumber
                        };
                        dataContext.Persons.InsertOnSubmit(dbPerson);
                    }
                    var dbReg = DAL.Part.PersonRegistration.FromXmlType(oioRegistration);
                    dbReg.Person = dbPerson;
                    dbReg.BrokerUpdateDate = DateTime.Now;
                    dataContext.PersonRegistrations.InsertOnSubmit(dbReg);
                    dataContext.SubmitChanges();
                    return true;
                }

            }
            return false;
        }

        public static void UpdatePersonUuid(string cprNumber, Guid uuid)
        {
            using (var dataContext = new PartDataContext())
            {
                PersonMapping map = new PersonMapping()
                {
                    CprNumber = cprNumber,
                    UUID = uuid
                };
                dataContext.PersonMappings.InsertOnSubmit(map);
                dataContext.SubmitChanges();
            }
        }

        public static void ImportPersonRegistrationFromXmlFile(string path)
        {
            var xml = System.IO.File.ReadAllText(path);
            var oio = DAL.Utilities.Deserialize<Schemas.Part.RegistreringType1>(xml);

            var uuid = new PersonIdentifier() { UUID = new Guid(path.Substring(path.LastIndexOf("\\") + 1)) };

            if (oio.AttributListe.RegisterOplysning[0].Item is CprBroker.Schemas.Part.CprBorgerType)
            {
                uuid.CprNumber = (oio.AttributListe.RegisterOplysning[0].Item as Schemas.Part.CprBorgerType).PersonCivilRegistrationIdentifier;
            }
            else if (oio.AttributListe.RegisterOplysning[0].Item is Schemas.Part.UdenlandskBorgerType)
            {
                uuid.CprNumber = (oio.AttributListe.RegisterOplysning[0].Item as Schemas.Part.UdenlandskBorgerType).PersonCivilRegistrationReplacementIdentifier;
            }
            else
            {
                uuid.CprNumber = (oio.AttributListe.RegisterOplysning[0].Item as Schemas.Part.UkendtBorgerType).PersonCivilRegistrationReplacementIdentifier;
            }
            UpdatePersonUuid(uuid.CprNumber, uuid.UUID.Value);
            UpdatePersonRegistration(uuid, oio);
        }
    }
}
