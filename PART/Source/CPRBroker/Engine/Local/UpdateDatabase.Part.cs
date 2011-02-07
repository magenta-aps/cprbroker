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
            /*
            dataContext.DataChangeEvents.InsertOnSubmit(pp);
            dataContext.SubmitChanges();

            NotificationQueueService.NotificationQueue notificationQueueService = new CprBroker.Engine.NotificationQueueService.NotificationQueue();
            notificationQueueService.Url = Config.Properties.Settings.Default.NotificationQueueServiceUrl;
            notificationQueueService.ApplicationHeaderValue = new CprBroker.Engine.NotificationQueueService.ApplicationHeader()
            {
                ApplicationToken = DAL.Applications.Application.BaseApplicationToken.ToString(),
                UserToken = Constants.UserToken
            };
            // TODO: use the value of the result
            bool result = notificationQueueService.Enqueue(personUuid);
             * */
        }

        private static bool MergePersonRegistration(PersonIdentifier personIdentifier, Schemas.Part.RegistreringType1 personRegistraion)
        {
            //TODO: Modify this method to allow searching for registrations that have a fake date of Today, these should be matched by content rather than registration date
            using (var dataContext = new PartDataContext())
            {
                DAL.Part.PersonRegistration.SetChildLoadOptions(dataContext);

                // Match db registrations by UUID, ActorId and registration date
                var existingInDb = (from dbReg in dataContext.PersonRegistrations
                                    where dbReg.UUID == personIdentifier.UUID
                                    && dbReg.RegistrationDate == TidspunktType.ToDateTime(personRegistraion.Tidspunkt)
                                    //TODO: Refill this condition
                                    //&& dbReg.ActorText == personRegistraion.AktoerTekst
                                    select dbReg).ToArray();

                var duplicateExists = existingInDb.Length > 0;

                // Perform a content match if key match is found
                if (duplicateExists)
                {
                    duplicateExists = Array.Exists(existingInDb, (db) => Schemas.Util.Misc.AreEqual<RegistreringType1>(DAL.Part.PersonRegistration.ToXmlType(db), personRegistraion));
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
                    var dbReg = DAL.Part.PersonRegistration.FromXmlType(personRegistraion);
                    dbReg.Person = dbPerson;
                    dbReg.BrokerUpdateDate = DateTime.Now;
                    dataContext.PersonRegistrations.InsertOnSubmit(dbReg);
                    dataContext.SubmitChanges();
                    return true;
                }

            }
            return false;
        }
    }
}
