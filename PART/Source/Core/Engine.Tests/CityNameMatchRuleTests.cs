using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine.UpdateRules;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Engine
{
    namespace CityNameMatchRuleTests
    {
        [TestFixture]
        public class GetObject
        {
            [Test]
            public void GetObject_Empty_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1());
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_EmptyAttr_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType() });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_NullOplysning_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = null } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_EmptyOplysning_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[0] } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_NonEmptyOplysningNull_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { null } } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_NonEmptyOplysningNullItem_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType { Item = null } } } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_UdellandsItem_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType { Item = new UdenlandskBorgerType() } } } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_UkendtItem_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType { Item = new UkendtBorgerType() } } } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_CpBorgerNullAddress_Null()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType { Item = new CprBorgerType { FolkeregisterAdresse = null } } } } });
                Assert.Null(ret);
            }

            [Test]
            public void GetObject_CpBorgerWithAddress_NotNull()
            {
                var rule = new CityNameMatchRule();
                var ret = rule.GetObject(new RegistreringType1() { AttributListe = new AttributListeType { RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType { Item = new CprBorgerType { FolkeregisterAdresse = new AdresseType { Item = new DanskAdresseType() } } } } } });
                Assert.NotNull(ret);
            }

        }
    }
}
