using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Tests.Schemas
{
    namespace PersonRelationTypeHelperTests
    {
        [TestFixture(typeof(PersonFlerRelationType))]
        [TestFixture(typeof(PersonRelationType))]
        public class Create_CprNumber<TRelation> where TRelation : IPersonRelationType, new()
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void Create_NullPnr_ThrowsException()
            {
                string pnr = null;
                PersonRelationTypeHelper.Create<TRelation>(pnr, null, null);
            }

            [Test]
            public void Create_Pnr_CorrectPnr()
            {
                string pnr = Utilities.RandomCprNumber();
                var ret = PersonRelationTypeHelper.Create<TRelation>(pnr, null, null);
                Assert.AreEqual(pnr, ret.CprNumber);
            }

            [Test]
            public void Create_Pnr_NullReferenceID()
            {
                string pnr = Utilities.RandomCprNumber();
                var ret = PersonRelationTypeHelper.Create<TRelation>(pnr, null, null);
                Assert.Null(ret.ReferenceID);
            }
        }
        
        [TestFixture(typeof(PersonFlerRelationType))]
        [TestFixture(typeof(PersonRelationType))]
        public class Create_Guid<TRelation> where TRelation : IPersonRelationType, new()
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void Create_EmptyUuid_ThrowsException()
            {
                Guid uuid = Guid.Empty;
                PersonRelationTypeHelper.Create<TRelation>(uuid, null, null);
            }

            [Test]
            public void Create_Uuid_CorrectUuid()
            {
                Guid uuid = Guid.NewGuid();
                var ret = PersonRelationTypeHelper.Create<TRelation>(uuid, null, null);
                Assert.AreEqual(uuid.ToString(), ret.ReferenceID.Item);
            }

            [Test]
            public void Create_Uuid_NullPnr()
            {
                Guid uuid = Guid.NewGuid();
                var ret = PersonRelationTypeHelper.Create<TRelation>(uuid, null, null);
                Assert.Null(ret.CprNumber);
            }
        }

        [TestFixture(typeof(PersonFlerRelationType))]
        [TestFixture(typeof(PersonRelationType))]
        public class CreateList_CprNumber<TRelation> where TRelation : IPersonRelationType, new()
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void CreateList_Null_ThrowsException()
            {
                string pnr = null;
                PersonRelationTypeHelper.CreateList<TRelation>(pnr);
            }

            [Test]
            public void Create_Pnr_CorrectPnr()
            {
                string pnr = Utilities.RandomCprNumber();
                var ret = PersonRelationTypeHelper.CreateList<TRelation>(pnr);
                Assert.AreEqual(pnr, ret[0].CprNumber);
            }

            [Test]
            public void Create_Pnr_NullReferenceID()
            {
                string pnr = Utilities.RandomCprNumber();
                var ret = PersonRelationTypeHelper.CreateList<TRelation>(pnr);
                Assert.Null(ret[0].ReferenceID);
            }
        }

        [TestFixture(typeof(PersonFlerRelationType))]
        [TestFixture(typeof(PersonRelationType))]
        public class CreateList_Uuid<TRelation> where TRelation : IPersonRelationType, new()
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void CreateList_Null_ThrowsException()
            {
                Guid uuid = Guid.Empty;
                PersonRelationTypeHelper.CreateList<TRelation>(uuid);
            }

            [Test]
            public void Create_Uuid_CorrectUuid()
            {
                Guid uuid = Guid.NewGuid();
                var ret = PersonRelationTypeHelper.CreateList<TRelation>(uuid);
                Assert.AreEqual(uuid.ToString(), ret[0].ReferenceID.Item);
            }

            [Test]
            public void Create_Uuid_NullCprNumber()
            {
                Guid uuid = Guid.NewGuid();
                var ret = PersonRelationTypeHelper.CreateList<TRelation>(uuid);
                Assert.Null(ret[0].CprNumber);
            }
        }

    }
}
