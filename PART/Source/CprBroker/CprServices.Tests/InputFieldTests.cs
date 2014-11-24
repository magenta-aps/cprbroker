using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CprServices;
using NUnit.Framework;
using System.Xml;

namespace CprBroker.Tests.CprServices
{
    namespace InputFieldTests
    {
        [TestFixture]
        public class _InputField
        {
            [Test]
            [ExpectedException]
            public void _InputField_Null_Exception()
            {
                new InputField(null);
            }

            [Test]
            [ExpectedException]
            public void _InputField_NoAttributes_Exception()
            {
                var doc = new XmlDocument();
                var node = doc.CreateElement("Field");
                new InputField(node);
            }

            [Test]
            public void _InputField_WithR_OK()
            {
                var doc = new XmlDocument();
                var node = doc.CreateElement("Field");
                var attr = doc.CreateAttribute("r");
                attr.Value = "afaf";
                node.Attributes.Append(attr);
                new InputField(node);
            }

            [Test]
            public void _InputField_Required_Required()
            {
                var doc = new XmlDocument();
                var node = doc.CreateElement("Field");

                var attr = doc.CreateAttribute("r");
                attr.Value = "afaf";
                node.Attributes.Append(attr);

                var req = doc.CreateAttribute("required");
                req.Value = "true";
                node.Attributes.Append(req);

                var ret = new InputField(node);
                Assert.True(ret.Required);
            }

            [Test]
            public void _InputField_RequiredGroup_Passed()
            {
                var doc = new XmlDocument();
                var node = doc.CreateElement("Field");

                var attr = doc.CreateAttribute("r");
                attr.Value = "afaf";
                node.Attributes.Append(attr);

                var gr = Guid.NewGuid().ToString();
                var req = doc.CreateAttribute("requiredGroup");
                req.Value = gr;
                node.Attributes.Append(req);

                var ret = new InputField(node);
                Assert.AreEqual(gr, ret.RequiredGroup);
            }
        }
    }
}
