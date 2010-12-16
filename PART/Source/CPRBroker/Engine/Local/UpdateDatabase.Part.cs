using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.DAL;
using CPRBroker.DAL.Part;


namespace CPRBroker.Engine.Local
{

    public partial class UpdateDatabase
    {
        /// <summary>
        /// Updates the system database with person registration objects
        /// </summary>
        /// <param name="personUUID"></param>
        /// <param name="personRegistraion"></param>
        public static void UpdatePersonRegistration(Guid personUUID, Schemas.Part.PersonRegistration personRegistraion)
        {
            //TODO: Modify this method to allow searching for registrations that have a fake date of Today, these should be matched by content rather than registration date
            using (var dataContext = new PartDataContext())
            {
                // Match db registrations by UUID, ActorId and registration date
                var existingInDb = (from dbReg in dataContext.PersonRegistrations
                                    where dbReg.UUID == personUUID
                                    && dbReg.RegistrationDate == personRegistraion.RegistrationDate
                                    && dbReg.ActorId == personRegistraion.ActorId
                                    select dbReg).ToArray();

                // Perform a content match if key match is found
                if (existingInDb.Length > 0)
                {
                    existingInDb = Array.FindAll(existingInDb, (db) => db.ToXmlType().Equals(personRegistraion));
                }
                // If there are really no matches, update the database
                if (existingInDb.Length == 0)
                {
                    var dbPerson = (from dbPers in dataContext.Persons
                                    where dbPers.UUID == personUUID
                                    select dbPers).FirstOrDefault();
                    if (dbPerson == null)
                    {
                        dbPerson = new CPRBroker.DAL.Part.Person()
                        {
                            UUID = personUUID
                        };
                        dataContext.Persons.InsertOnSubmit(dbPerson);
                    }
                    var dbReg = DAL.Part.PersonRegistration.FromXmlType(personRegistraion);
                    dbReg.Person = dbPerson;
                    dataContext.PersonRegistrations.InsertOnSubmit(dbReg);
                    dataContext.SubmitChanges();
                }
            }
        }
    }
}
