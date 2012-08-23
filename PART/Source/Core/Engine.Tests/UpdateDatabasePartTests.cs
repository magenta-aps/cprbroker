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

namespace CprBroker.Tests.Engine
{
    namespace UpdateDatabase.Part.Tests
    {
        [TestFixture]
        public class EnsurePersonExists
        {
            [Test]
            public void EnsurePersonExists_New_Saved()
            {
                var uuid = Guid.NewGuid();
                var pnr = Utilities.RandomCprNumber();

                using (var dataContext = new PartDataContext())
                {

                    var person = CprBroker.Engine.Local.UpdateDatabase.EnsurePersonExists(dataContext, new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = uuid });
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

            [Test]
            public void EnsurePersonExists_Existing_Saved(
                [Random(0, 99, 10)] int index)
            {
                using (var dataContext = new PartDataContext())
                {
                    var dbPerson = dataContext.Persons.Skip(index).First();

                    var uuid = dbPerson.UUID;
                    var pnr = dbPerson.UserInterfaceKeyText;

                    var person = CprBroker.Engine.Local.UpdateDatabase.EnsurePersonExists(dataContext, new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = uuid });
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
        public class InsertPerson
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
        public class MergePersonRegistration
        {
            [Test]
            public void MergePersonRegistration_FromExisting_False(
                [Random(0, 99, 10)] int index)
            {
                using (var dataContext = new PartDataContext())
                {
                    var dbPerson = dataContext.PersonRegistrations.Where(pr => pr.Person != null).Skip(index).First();
                    var oio = PersonRegistration.ToXmlType(dbPerson);
                    var pId = new PersonIdentifier() { UUID = dbPerson.UUID, CprNumber = dbPerson.Person.UserInterfaceKeyText };
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio);
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
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio);
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
                    var ret = CprBroker.Engine.Local.UpdateDatabase.MergePersonRegistration(pId, oio);
                    Assert.True(ret);

                    var dbPerson = dataContext.PersonRegistrations.Where(pr => pr.UUID == pId.UUID).First();
                    var tmpElement = System.Xml.Linq.XElement.Parse(oio.SourceObjectsXml);

                    Assert.AreEqual(tmpElement.ToString(), dbPerson.SourceObjects.ToString());
                }
            }


        }

    }
}
