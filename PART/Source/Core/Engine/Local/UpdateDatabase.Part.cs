/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Data;
using CprBroker.Data.Events;
using CprBroker.Data.Part;
using System.Xml.Linq;

namespace CprBroker.Engine.Local
{
    /// <summary>
    /// Updates the local database with new data versions from external data providers
    /// </summary>
    public partial class UpdateDatabase
    {
        /// <summary>
        /// Updates the system database with person registration objects
        /// </summary>
        /// <param name="personIdentifier"></param>
        /// <param name="personRegistraion"></param>
        public static void UpdatePersonRegistration(PersonIdentifier personIdentifier, Schemas.Part.RegistreringType1 personRegistraion)
        {
            // TODO: differentiate the 'true' returned between cases when new data is inserted or just SourceObjects is updated
            Guid? personRegistrationId = null;
            if (MergePersonRegistration(personIdentifier, personRegistraion, out personRegistrationId))
            {
                // TODO: move this call to a separate phase in request processing
                NotifyPersonRegistrationUpdate(personIdentifier.UUID.Value, personRegistrationId.Value);
            }
        }

        private static void NotifyPersonRegistrationUpdate(Guid personUuid, Guid personRegistrationId)
        {
            using (var dataContext = new DataChangeEventDataContext())
            {
                var pp = new DataChangeEvent()
                {
                    DataChangeEventId = Guid.NewGuid(),
                    PersonUuid = personUuid,
                    PersonRegistrationId = personRegistrationId,
                    ReceivedDate = DateTime.Now
                };
                dataContext.DataChangeEvents.InsertOnSubmit(pp);
                dataContext.SubmitChanges();
            }
        }

        public static bool MergePersonRegistration(PersonIdentifier personIdentifier, Schemas.Part.RegistreringType1 oioRegistration, out Guid? personRegistrationId)
        {
            using (var dataContext = new PartDataContext())
            {
                // Load possible equal registrations
                bool dataChanged;
                var existingInDb = MatchPersonRegistration(personIdentifier, oioRegistration, dataContext, out dataChanged);
                Func<PersonRegistration[], Guid?> latestRegistrationFunc = (dbRegs) =>
                    {
                        if (dbRegs.Count() > 0)
                        {
                            return dbRegs.OrderByDescending(db => db.RegistrationDate).ThenByDescending(db => db.BrokerUpdateDate).Select(db => db.PersonRegistrationId).FirstOrDefault();
                        }
                        return null;
                    };

                // If key & contents match was not found
                if (existingInDb.Length == 0)
                {
                    var dbPerson = EnsurePersonExists(dataContext, personIdentifier);
                    var dbReg = InsertPerson(dataContext, dbPerson, oioRegistration);
                    dataContext.SubmitChanges();

                    personRegistrationId = dbReg.PersonRegistrationId;
                    return true;
                }
                else // key & content match found
                {
                    if (string.IsNullOrEmpty(oioRegistration.SourceObjectsXml))
                    {
                        // No need to update anything - just commit if needed
                        if (dataChanged)
                        {
                            dataContext.SubmitChanges();
                            personRegistrationId = latestRegistrationFunc(existingInDb);
                        }
                        return dataChanged;
                    }
                    else
                    {
                        var existinginDbWithoutSource = existingInDb.Where(db => db.SourceObjects == null || db.SourceObjects.IsEmpty).ToArray();

                        // If existing registration(s) has exactly same keys and contents , with empty SourceObjects, update source objects
                        if (existinginDbWithoutSource.Length > 0)
                        {
                            Array.ForEach<PersonRegistration>(
                                existinginDbWithoutSource,
                                db => db.SourceObjects = XElement.Parse(oioRegistration.SourceObjectsXml));
                            dataContext.SubmitChanges();
                            personRegistrationId = latestRegistrationFunc(existinginDbWithoutSource);
                            return true;
                        }
                        else
                        {
                            var xml = XElement.Parse(oioRegistration.SourceObjectsXml).ToString();
                            var existinginDbWithEqualSource = existingInDb
                                .Where(db =>
                                    db.SourceObjects != null
                                    && !db.SourceObjects.IsEmpty
                                    && db.SourceObjects.ToString().Equals(xml))
                                .ToArray();

                            // If existing registration has exactly same keys and contents and SourceObjects, return without doing anything - no need to update
                            if (existinginDbWithEqualSource.Count() > 0)
                            {
                                // Already up to date - just save updated records if needed
                                if (dataChanged)
                                {
                                    dataContext.SubmitChanges();
                                    personRegistrationId = latestRegistrationFunc(existinginDbWithEqualSource);
                                }
                                return dataChanged;
                            }
                            else
                            {
                                // Otherwise : insert new registration
                                var dbPerson = EnsurePersonExists(dataContext, personIdentifier);
                                var dbReg = InsertPerson(dataContext, dbPerson, oioRegistration);
                                dataContext.SubmitChanges();
                                personRegistrationId = dbReg.PersonRegistrationId;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        private static PersonRegistration[] MatchPersonRegistration(PersonIdentifier personIdentifier, RegistreringType1 oioRegistration, PartDataContext dataContext, out bool dataChanged)
        {
            //TODO: Modify this method to allow searching for registrations that have a fake date of Today, these should be matched by content rather than registration date

            dataChanged = false;
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

            var ret = new List<PersonRegistration>(existingInDb.Length);

            // Perform a content match if key match is found
            foreach (var dbReg in existingInDb)
            {
                if (dbReg.Equals(oioRegistration))
                {
                    // Exact content match
                    ret.Add(dbReg);
                }
                else
                {
                    // Content match due to bug fix
                    if (UpdateRules.MatchRule.ApplyRules(dbReg, oioRegistration))
                    {
                        dataChanged = true;
                        ret.Add(dbReg);
                    }
                }
            }
            return ret.ToArray();
        }


        public static Person EnsurePersonExists(PartDataContext dataContext, PersonIdentifier personIdentifier)
        {
            var dbPerson = (from dbPers in dataContext.Persons
                            where dbPers.UUID == personIdentifier.UUID
                            select dbPers).FirstOrDefault();
            if (dbPerson == null)
            {
                dbPerson = new CprBroker.Data.Part.Person()
                {
                    UUID = personIdentifier.UUID.Value,
                    UserInterfaceKeyText = personIdentifier.CprNumber
                };
                dataContext.Persons.InsertOnSubmit(dbPerson);
            }
            return dbPerson;
        }

        public static PersonRegistration InsertPerson(PartDataContext dataContext, Person dbPerson, RegistreringType1 oioRegistration)
        {
            var dbReg = Data.Part.PersonRegistration.FromXmlType(oioRegistration);
            dbReg.Person = dbPerson;
            dbReg.BrokerUpdateDate = DateTime.Now;
            dataContext.PersonRegistrations.InsertOnSubmit(dbReg);
            return dbReg;
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

        public static void UpdatePersonUuidArray(string[] cprNumbers, Guid?[] uuids)
        {
            using (var dataContext = new PartDataContext())
            {
                var all = new List<PersonMapping>(cprNumbers.Length);
                for (int i = 0; i < cprNumbers.Length; i++)
                {
                    var uuid = uuids[i];
                    if (uuid.HasValue)
                    {
                        all.Add(new PersonMapping()
                            {
                                CprNumber = cprNumbers[i],
                                UUID = uuid.Value
                            });
                    }
                }

                dataContext.PersonMappings.InsertAllOnSubmit(all);
                dataContext.SubmitChanges();
            }
        }

        public static void ImportPersonRegistrationFromXmlFile(string path)
        {
            var xml = System.IO.File.ReadAllText(path);
            var oio = Utilities.Strings.Deserialize<Schemas.Part.RegistreringType1>(xml);

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
