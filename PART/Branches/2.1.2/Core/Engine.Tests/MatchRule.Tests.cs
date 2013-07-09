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

            public Action<T, T> _UpdateOioFromXmlType;
            public override void UpdateOioFromXmlType(T existingObj, T newObj)
            {
                if (_UpdateOioFromXmlType != null)
                    _UpdateOioFromXmlType(existingObj, newObj);
            }

            public Action<PersonRegistration, T> _UpdateDbFromXmlType;
            public override void UpdateDbFromXmlType(PersonRegistration dbReg, T newObj)
            {
                if (_UpdateDbFromXmlType != null)
                    _UpdateDbFromXmlType(dbReg, newObj);
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
        public class UpdateXmlTypeIfPossible
        {
            [Test]
            public void UpdateXmlTypeIfPossible_NoCandidates_False()
            {
                var oioReg0 = new RegistreringType1();
                var oioReg1 = new RegistreringType1();

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = false, _GetObject = null, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg0, oioReg1);
                Assert.False(ret);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_NoCandidates_NoChanges()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                var xml1 = Strings.SerializeObject(dbReg);

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = false, _GetObject = null, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg);
                var xml2 = Strings.SerializeObject(dbReg);
                Assert.AreEqual(xml1, xml2);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_Candidates_True()
            {
                var oioReg = new RegistreringType1();

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObject = oioReg, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg);
                Assert.True(ret);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_Candidates_MethodCall()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);
                bool ff = false;
                var rule = new DummyRule<object> { _AreCandidates = true, _GetObject = "", _UpdateOioFromXmlType = (r1, r2) => ff = true };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg);
                Assert.True(ff);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_Candidates_DataChanged()
            {
                var oioReg0 = new RegistreringType1();
                var oioReg1 = new RegistreringType1();

                var xml1 = Strings.SerializeObject(oioReg0);

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObjectM = o => o, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg0, oioReg1);
                var xml2 = Strings.SerializeObject(oioReg0);
                Assert.AreNotEqual(xml1, xml2);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_CandidatesWithNullObjects_False()
            {
                var oioReg = new RegistreringType1();
                var dbReg = new PersonRegistration();
                dbReg.SetContents(oioReg);

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObject = null, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg);
                Assert.False(ret);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_CandidatesWithNullObjects_SameData()
            {
                var oioReg0 = new RegistreringType1();
                var oioReg1 = new RegistreringType1();
                var xml1 = Strings.SerializeObject(oioReg0);

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObject = null, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg0, oioReg1);
                var xml2 = Strings.SerializeObject(oioReg0);
                Assert.AreEqual(xml1, xml2);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_CandidatesWithOneNullObject_False([Values(0, 1)]int callNumber)
            {
                var oioReg = new RegistreringType1();
                var oioReg2 = new RegistreringType1();
                var oioRegs = new RegistreringType1[] { oioReg, oioReg2 };

                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObjectM = (o) => o == oioRegs[callNumber] ? null : o, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg2);
                Assert.False(ret);
            }

            [Test]
            public void UpdateXmlTypeIfPossible_CandidatesWithOneNullObject_FalseAndSameData([Values(0, 1)]int callNumber)
            {
                var oioReg = new RegistreringType1();
                var oioReg2 = new RegistreringType1();
                var oioRegs = new RegistreringType1[] { oioReg, oioReg2 };

                var xml1 = Strings.SerializeObject(oioReg);
                var rule = new DummyRule<RegistreringType1> { _AreCandidates = true, _GetObjectM = (o) => o == oioRegs[callNumber] ? null : o, _UpdateOioFromXmlType = (r1, r2) => r1.AttributListe = new AttributListeType() { } };
                var ret = rule.UpdateXmlTypeIfPossible(oioReg, oioReg2);

                var xml2 = Strings.SerializeObject(oioReg);
                Assert.AreEqual(xml1, xml2);
            }
        }

        [TestFixture]
        public class UpdateDbFromXmlType
        {
            [Test]
            public void UpdateDbFromXmlType_Normal_CallsLowerFunc()
            {
                bool called = false;
                var rule = new DummyRule<RegistreringType1>() { _UpdateDbFromXmlType = (db, oio) => called = true };
                rule.UpdateDbFromXmlType(new PersonRegistration(), new RegistreringType1());
                Assert.True(called);
            }
        }
    }
}
