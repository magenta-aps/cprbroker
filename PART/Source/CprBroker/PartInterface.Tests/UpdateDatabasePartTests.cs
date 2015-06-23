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
using NUnit.Framework;
using CprBroker.Data.Part;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.PartInterface
{
    namespace UpdateDatabase.Part.Tests
    {
        [TestFixture]
        public class EnsurePersonExists : TestBase
        {
            [Test]
            public void EnsurePersonExists_New_Saved()
            {
                var uuid = Guid.NewGuid();
                var pnr = Utilities.RandomCprNumber();

                using (var dataContext = new PartDataContext())
                {
                    var person = CprBroker.Engine.Local.UpdateDatabase.EnsurePersonExists(dataContext, new CprBroker.Schemas.PersonIdentifier() { CprNumber = pnr, UUID = uuid });
                    Assert.NotNull(person);
                    Assert.AreEqual(uuid, person.UUID);
                    Assert.AreEqual(pnr, person.UserInterfaceKeyText);

                    Assert.AreEqual(1, dataContext.GetChangeSet().Inserts.Count);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Updates.Count);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Deletes.Count);
                    dataContext.SubmitChanges();
                }

                using (var dataContext = new PartDataContext())
                {
                    var person = dataContext.Persons.Where(p => p.UUID == uuid).First();
                    Assert.NotNull(person);
                    Assert.AreEqual(pnr, person.UserInterfaceKeyText);
                }
            }

            void AddTestPerson(int count)
            {
                using (var dataContext = new PartDataContext())
                {
                    for (int i = 0; i < count; i++)
                    {
                        var dataPerson = new Person() { UUID = Guid.NewGuid(), UserInterfaceKeyText = Utilities.RandomCprNumber() };
                        dataContext.Persons.InsertOnSubmit(dataPerson);
                    }
                    dataContext.SubmitChanges();
                }
            }

            [Test]
            public void EnsurePersonExists_Existing_Saved(
                [Random(0, 99, 10)] int index)
            {
                AddTestPerson(110);

                using (var dataContext = new PartDataContext())
                {
                    var dbPerson = dataContext.Persons.Skip(index).First();

                    var uuid = dbPerson.UUID;
                    var pnr = dbPerson.UserInterfaceKeyText;

                    var person = CprBroker.Engine.Local.UpdateDatabase.EnsurePersonExists(dataContext, new CprBroker.Schemas.PersonIdentifier() { CprNumber = pnr, UUID = uuid });
                    Assert.NotNull(person);
                    Assert.AreEqual(uuid, person.UUID);
                    Assert.AreEqual(pnr, person.UserInterfaceKeyText);

                    Assert.AreEqual(0, dataContext.GetChangeSet().Inserts.Count);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Updates.Count);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Deletes.Count);
                }
            }
        }

        [TestFixture]
        public class LockTests
        {
            [Test]
            public void Locking()
            {

                var inp = new string[]{
                    "" + 2233.ToString(),
                    2.ToString() + 233.ToString(),
                    22.ToString() + 33.ToString(),
                    223.ToString() + 3.ToString(),
                    2233.ToString() + ""
                };
                inp = inp.Select(o => String.Intern(o)).ToArray();
                bool failed = false;
                long running = 0;
                
                Action<int> func = (int index) =>
                {
                    var s = inp[index];
                    lock (s)
                    {
                        if (System.Threading.Interlocked.Read(ref running) > 0)
                            failed = true;
                        Console.WriteLine("{0} Entering Mutex {1}", DateTime.Now, s);
                        System.Threading.Interlocked.Increment(ref running);
                        System.Threading.Thread.Sleep(5000);
                        System.Threading.Interlocked.Decrement(ref running);
                        Console.WriteLine("{0} Exiting Mutex {1}", DateTime.Now, s);
                    }
                };

                System.Threading.Thread[] threads =
                    inp.Select(i =>
                        new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((o) => func((int)o)))
                        )
                        .ToArray();

                int bigIndex = 0;
                foreach (var th in threads)
                    th.Start(bigIndex++);

                System.Threading.Thread.Sleep(inp.Length * 5000);
                Assert.False(failed);
            }
        }

        [TestFixture]
        public class InsertPerson : TestBase
        {
            [Test]
            public void InsertPerson_New_CorrectOutput()
            {
                using (var dataContext = new PartDataContext())
                {
                    var dbPerson = new Person() { UserInterfaceKeyText = Utilities.RandomCprNumber(), UUID = Guid.NewGuid() };
                    var oioRegistration = Utilities.CreateFakePerson();
                    var newDbRegistration = CprBroker.Engine.Local.UpdateDatabase.InsertPerson(dataContext, dbPerson, oioRegistration);

                    Assert.Greater(dataContext.GetChangeSet().Inserts.Count, 0);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Updates.Count);
                    Assert.AreEqual(0, dataContext.GetChangeSet().Deletes.Count);

                    dataContext.SubmitChanges();
                }
            }
        }

        [TestFixture]
        public class MergePersonRegistration : TestBase
        {
            void AddPersons(int count)
            {
                using (var dataContext = new PartDataContext())
                {
                    var arOio = UnikIdType.Create(Guid.NewGuid());
                    var ar = ActorRef.FromXmlType(arOio);
                    dataContext.ActorRefs.InsertOnSubmit(ar);
                    dataContext.SubmitChanges();

                    for (int i = 0; i < count; i++)
                    {
                        var p = new Person() { UserInterfaceKeyText = Utilities.RandomCprNumber(), UUID = Guid.NewGuid() };
                        dataContext.Persons.InsertOnSubmit(p);



                        var pr = new PersonRegistration()
                        {
                            PersonRegistrationId = Guid.NewGuid(),
                            BrokerUpdateDate = DateTime.Now,
                            RegistrationDate = DateTime.Now,
                            LifecycleStatusId = 1,
                            ActorRef = ar,
                        };
                        pr.SetContents(new RegistreringType1()
                        {
                            Tidspunkt = new TidspunktType() { Item = DateTime.Now },
                            AktoerRef = arOio,
                        });
                        p.PersonRegistrations.Add(pr);
                        dataContext.PersonRegistrations.InsertOnSubmit(pr);
                    }
                    dataContext.SubmitChanges();
                }
            }
            Guid? personRegistrationId;
            [Test]
            public void MergePersonRegistration_FromExisting_False(
                [Random(0, 99, 10)] int index)
            {
                AddPersons(110);
                using (var dataContext = new PartDataContext())
                {
                    var dbPerson = dataContext.PersonRegistrations.Where(pr => pr.Person != null).Skip(index).First();
                    var oio = PersonRegistration.ToXmlType(dbPerson);
                    var pId = new PersonIdentifier() { UUID = dbPerson.UUID, CprNumber = dbPerson.Person.UserInterfaceKeyText };
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.False(ret);
                }
            }

            [Test]
            public void MergePersonRegistration_New_True(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    var oio = Utilities.CreateFakePerson();
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.True(ret);
                }
            }

            [Test]
            public void MergePersonRegistration_NewWithSource_CorrectSource(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    var oio = Utilities.CreateFakePerson(true);
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.True(ret);

                    var dbPerson = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).FirstOrDefault();
                    Assert.NotNull(dbPerson);

                    var tmpElement = System.Xml.Linq.XElement.Parse(oio.SourceObjectsXml);
                    Assert.AreEqual(tmpElement.ToString(), dbPerson.SourceObjects.ToString());
                }
            }

            [Test]
            public void MergePersonRegistration_ExistsWithDifferentSource_NewRecordAdded(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    // Insert old record
                    var oio = Utilities.CreateFakePerson(true);
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);


                    // Set new XML source
                    oio.SourceObjectsXml = Utilities.CreateFakePerson(true).SourceObjectsXml;
                    var newRet = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.True(newRet);

                    var dbPersons = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).OrderBy(p => p.BrokerUpdateDate).ToArray();
                    Assert.AreEqual(2, dbPersons.Length);

                    var tmpElement = System.Xml.Linq.XElement.Parse(oio.SourceObjectsXml);
                    Assert.AreEqual(tmpElement.ToString(), dbPersons[1].SourceObjects.ToString());
                }
            }

            [Test]
            public void MergePersonRegistration_ExistsWithoutSource_SourceUpdated(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    // Insert old record
                    var oio = Utilities.CreateFakePerson(false);
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);


                    // Set new XML source
                    oio.SourceObjectsXml = Utilities.CreateFakePerson(true).SourceObjectsXml;
                    var newRet = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.True(newRet);

                    var dbPersons = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).OrderBy(p => p.BrokerUpdateDate).ToArray();
                    Assert.AreEqual(1, dbPersons.Length);

                    var tmpElement = System.Xml.Linq.XElement.Parse(oio.SourceObjectsXml);
                    Assert.AreEqual(tmpElement.ToString(), dbPersons[0].SourceObjects.ToString());
                }
            }

            [Test]
            public void MergePersonRegistration_ExistsWithSameSource_NoUpdate(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    // Insert old record
                    var oio = Utilities.CreateFakePerson(true);
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);


                    var newRet = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.False(newRet);

                    var dbPersons = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).OrderBy(p => p.BrokerUpdateDate).ToArray();
                    Assert.AreEqual(1, dbPersons.Length);

                    var tmpElement = System.Xml.Linq.XElement.Parse(oio.SourceObjectsXml);
                    Assert.AreEqual(tmpElement.ToString(), dbPersons[0].SourceObjects.ToString());
                }
            }

            [Test]
            public void MergePersonRegistration_ExistsWithSource_NewWithouSource_NoUpdate(
                [Range(1, 10)] int dummy)
            {
                using (var dataContext = new PartDataContext())
                {
                    // Insert old record
                    var oio = Utilities.CreateFakePerson(true);
                    var pId = new PersonIdentifier() { UUID = Guid.NewGuid(), CprNumber = Utilities.RandomCprNumber() };
                    CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);

                    var xml = oio.SourceObjectsXml;
                    oio.SourceObjectsXml = null;
                    var newRet = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio, out personRegistrationId);
                    Assert.False(newRet);

                    var dbPersons = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).OrderBy(p => p.BrokerUpdateDate).ToArray();
                    Assert.AreEqual(1, dbPersons.Length);

                    var tmpElement = System.Xml.Linq.XElement.Parse(xml);
                    Assert.AreEqual(tmpElement.ToString(), dbPersons[0].SourceObjects.ToString());
                }
            }


        }

    }
}
