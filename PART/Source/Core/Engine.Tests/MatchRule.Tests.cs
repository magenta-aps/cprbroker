using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine.UpdateRules;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;
using CprBroker.Utilities;

namespace CprBroker.Tests.Engine
{
    namespace MatchRuleTests
    {
        class DummyRule<T> : MatchRule<T>
        {
            public T _GetObject = default(T);

            public Func<RegistreringType1, T> _GetObjectM = null;

            public override T GetObject(RegistreringType1 oio)
            {
                if (_GetObjectM != null)
                    return _GetObjectM(oio);
                else
                    return _GetObject;
            }

            public bool _AreCandidates;
            public override bool AreCandidates(T existingObj, T newObj)
            {
                return _AreCandidates;
            }

            public Action<PersonRegistration, T, T> _UpdateFromXmlType;
            public override void UpdateFromXmlType(PersonRegistration dbReg, T existingObj, T newObj)
            {
                if (_UpdateFromXmlType != null)
                    _UpdateFromXmlType(dbReg, existingObj, newObj);
            }
        }

        [TestFixture]
        public class AllRules
        {
            [Test]
            public void AllRules_Call_NotNullOrEmpty()
            {
                var rules = MatchRule.AllRules();
                Assert.IsNotEmpty(rules);
            }

            [Test]
            public void AllRules_Call_FirstNotNull()
            {
                var rules = MatchRule.AllRules();
                var first = rules.FirstOrDefault();
                Assert.NotNull(first);
            }

            [Test]
            public void AllRules_Call_ContainsCityName()
            {
                var rules = MatchRule.AllRules().Where(r => r is CityNameMatchRule);
                Assert.Greater(rules.Count(), 0);
            }
        }

        [TestFixture]
        public class ApplyRules
        {
            [Test]
            public void ApplyRules_Empty_False()
            {
                var dbReg = new PersonRegistration();
                var oioReg = new RegistreringType1();
                dbReg.SetContents(oioReg);
                var ret = MatchRule.ApplyRules(dbReg, oioReg, new MatchRule[0]);
                Assert.False(ret);
            }

            [Test]
            public void ApplyRules_Empty_SameObject()
            {
                var dbReg = new PersonRegistration();
                var oioReg = new RegistreringType1();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);
                var ret = MatchRule.ApplyRules(dbReg, oioReg, new MatchRule[0]);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }

            [Test]
            public void ApplyRules_IrrelevantRules_SameObject()
            {
                var dbReg = new PersonRegistration();
                var oioReg = new RegistreringType1();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);
                var ret = MatchRule.ApplyRules(dbReg, oioReg);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }
        }

        [TestFixture]
        public class UpdateFromXmlIfPossible
        {
            [Test]
            public void UpdateFromXmlIfPossible_NoCandidates_False()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);

                var rule = new DummyRule<object> { _AreCandidates = false, _GetObject = "", _UpdateFromXmlType = (db, r1, r2) => db.PersonAttributes = new PersonAttributes() { } };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                Assert.False(ret);
            }

            [Test]
            public void UpdateFromXmlIfPossible_NoCandidates_NoChanges()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);

                var rule = new DummyRule<object> { _AreCandidates = false, _GetObject = "", _UpdateFromXmlType = (db, r1, r2) => db.PersonAttributes = new PersonAttributes() { } };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }

            [Test]
            public void UpdateFromXmlIfPossible_Candidates_True()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);

                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = "", _UpdateFromXmlType = (db, r1, r2) => db.PersonAttributes = new PersonAttributes() { } };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                Assert.True(ret);
            }

            [Test]
            public void UpdateFromXmlIfPossible_Candidates_MethodCall()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                bool ff = false;
                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = "", _UpdateFromXmlType = (db, r1, r2) => ff = true };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                Assert.True(ff);
            }

            [Test]
            public void UpdateFromXmlIfPossible_Candidates_DataChanged()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);

                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = "", _UpdateFromXmlType = (db, r1, r2) => db.UUID = Guid.NewGuid() };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreNotEqual(xml1, xml2);
            }

            [Test]
            public void UpdateFromXmlIfPossible_CandidatesWithNullObjects_False()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);

                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = null, _UpdateFromXmlType = (db, r1, r2) => db.UUID = Guid.NewGuid() };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                Assert.False(ret);
            }

            [Test]
            public void UpdateFromXmlIfPossible_CandidatesWithNullObjects_SameData()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);

                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = null, _UpdateFromXmlType = (db, r1, r2) => db.UUID = Guid.NewGuid() };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }

            [Test]            
            public void UpdateFromXmlIfPossible_CandidatesWithOneNullObject_False([Values(0,1)]int callNumber)
            {
                var oioReg = new RegistreringType1();
                var oioReg2 = new RegistreringType1();
                var oioRegs = new RegistreringType1[]{ oioReg,oioReg2};
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var rule = new DummyRule<object> { _AreCandidates = true, _GetObjectM = (o) => o == oioRegs[callNumber] ? null : new object(), _UpdateFromXmlType = (db, r1, r2) => db.UUID = Guid.NewGuid() };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg2);
                Assert.False(ret);
            }

            [Test]
            public void UpdateFromXmlIfPossible_CandidatesWithOneNullObject_FalseAndSameData([Values(0, 1)]int callNumber)
            {
                var oioReg = new RegistreringType1();
                var oioReg2 = new RegistreringType1();
                var oioRegs = new RegistreringType1[] { oioReg, oioReg2 };
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);
                var rule = new DummyRule<object> { _AreCandidates = true, _GetObjectM = (o) => o == oioRegs[callNumber] ? null : new object(), _UpdateFromXmlType = (db, r1, r2) => db.UUID = Guid.NewGuid() };
                var ret = rule.UpdateFromXmlIfPossible(dbReg, oioReg, oioReg2);
                
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }
        }
    }
}
