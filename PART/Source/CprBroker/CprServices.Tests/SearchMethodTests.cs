using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CprServices;
using NUnit.Framework;

namespace CprBroker.Tests.CprServices
{
    public class SearchMethodTestsBase
    {
        public string[] AvailableInputs
        {
            get
            {
                return new string[]{
                        CprBroker.Providers.CprServices.Properties.Resources.ADRSOG1,
                        CprBroker.Providers.CprServices.Properties.Resources.ADRESSE3,
                        CprBroker.Providers.CprServices.Properties.Resources.NVNSOG2
                    };
            }
        }

        public string SearchTemplate { get { return CprBroker.Providers.CprServices.Properties.Resources.SearchTemplate; } }
    }


    namespace SearchMethodTests
    {
        
        [TestFixture]
        public class _SearchMethod
        {
            [TestCaseSource(typeof(SearchMethodTestsBase), "AvailableInputs")]
            public void _SearchMethod_AvailableInputs_NoException(string xmlTemplate)
            {
                var method = new SearchMethod(xmlTemplate);
            }

            [TestCaseSource(typeof(SearchMethodTestsBase), "AvailableInputs")]
            public void _SearchMethod_AvailableInputs_HasName(string xmlTemplate)
            {
                var method = new SearchMethod(xmlTemplate);
                Assert.IsNotNullOrEmpty(method.Name);
            }

            [TestCaseSource(typeof(SearchMethodTestsBase), "AvailableInputs")]
            public void _SearchMethod_AvailableInputs_HasFields(string xmlTemplate)
            {
                var method = new SearchMethod(xmlTemplate);
                Assert.IsNotEmpty(method.InputFields);
            }
        }

        [TestFixture]
        public class CanBeUsedFor
        {
            [Test]
            public void CanBeUsedFor_AllEmpty_True()
            {
                var m = new SearchMethod();
                var r = new SearchRequest(new CprBroker.Schemas.Part.SoegAttributListeType());
                var ret = m.CanBeUsedFor(r);
                Assert.True(ret);
            }

            [Test]
            public void CanBeUsedFor_NoRequired_NonEmptyRequest_True()
            {
                var m = new SearchMethod();
                var r = new SearchRequest(new CprBroker.Schemas.Part.SoegAttributListeType());
                r.AddCriteriaField("afjajf", "asdfadf");
                var ret = m.CanBeUsedFor(r);
                Assert.True(ret);
            }

            [Test]
            public void CanBeUsedFor_RequiredParameterMissing_False()
            {
                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DD", Required = true });
                var r = new SearchRequest(new CprBroker.Schemas.Part.SoegAttributListeType());
                var ret = m.CanBeUsedFor(r);
                Assert.False(ret);
            }

            [Test]
            public void CanBeUsedFor_RequiredGroup_AllMissing_False()
            {
                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DD", Required = true, RequiredGroup = "G" });
                m.InputFields.Add(new InputField() { Name = "DD2", Required = true, RequiredGroup = "G" });
                var r = new SearchRequest(new CprBroker.Schemas.Part.SoegAttributListeType());
                var ret = m.CanBeUsedFor(r);
                Assert.False(ret);
            }

            [Test]
            public void CanBeUsedFor_RequiredGroup_OneAvailable_True()
            {
                var m = new SearchMethod();
                m.InputFields.Add(new InputField() { Name = "DD", Required = true, RequiredGroup = "G" });
                m.InputFields.Add(new InputField() { Name = "DD2", Required = true, RequiredGroup = "G" });
                var r = new SearchRequest(new CprBroker.Schemas.Part.SoegAttributListeType());
                r.AddCriteriaField("DD", "qwwd");
                var ret = m.CanBeUsedFor(r);
                Assert.True(ret);
            }
        }
    }
}
