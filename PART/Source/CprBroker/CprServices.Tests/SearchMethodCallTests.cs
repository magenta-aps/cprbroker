using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CprServices;
using System.Xml;

namespace CprBroker.Tests.CprServices
{
    namespace SearchMethodCallTests
    {
        [TestFixture]
        public class ToRequestXml
        {
            [TestCaseSource(typeof(SearchMethodTestsBase), "AvailableInputs")]
            public void ToRequestXml_NoFields_SameAsInput(string template)
            {
                var doc = new XmlDocument();
                doc.LoadXml(template);
                var name = doc.SelectSingleNode("//CprServiceHeader").Attributes["r"].Value;

                var mc = new SearchMethodCall() { Name = name };
                var ret = mc.ToRequestXml(template);

                var doc2 = new XmlDocument();
                doc2.LoadXml(ret);
                Assert.AreEqual(doc.OuterXml, doc2.OuterXml);
            }

            [TestCaseSource(typeof(SearchMethodTestsBase), "AvailableInputs")]
            public void ToRequestXml_OneFields_LongerThanInput(string template)
            {
                var doc = new XmlDocument();
                doc.LoadXml(template);
                var name = doc.SelectSingleNode("//CprServiceHeader").Attributes["r"].Value;

                var mc = new SearchMethodCall() { Name = name };
                mc.InputFields.Add(new KeyValuePair<string, string>("DDD", "DDDD"));
                var ret = mc.ToRequestXml(template);

                var doc2 = new XmlDocument();
                doc2.LoadXml(ret);
                Assert.Greater(doc2.OuterXml.Length, doc.OuterXml.Length);
            }
        }
    }
}
