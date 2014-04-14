using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Installers;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.Tests.Installers
{
    namespace InstallationTests
    {
        [TestFixture]
        public class CopyConfigNode
        {
            [Test]
            public void CopyConfigNode_Overwrite_OK()
            {
                var sourceName = "Src" + Strings.NewRandomString(5);
                var targetName = "Target" + Strings.NewRandomString(5);

                File.WriteAllText(sourceName, Properties.Resources.WebConfig);
                File.WriteAllText(targetName, Properties.Resources.CprBroker_EventBroker_Backend_exe);

                Installation.CopyConfigNode("configuration", "configSections", sourceName, targetName, Installation.MergeOption.Overwrite);

                File.Delete(sourceName);
                File.Delete(targetName);
            }
        }

        [TestFixture]
        public class AddSectionNode
        {
            [Test]
            public void AddSectionNode_New_OK()
            {
                var nodeName = "S" + Guid.NewGuid().ToString();
                string fileName = "F" + Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(fileName, Properties.Resources.WebConfig);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                Installation.AddSectionNode(nodeName, dic, "klfajsklj", fileName, "//configuration");

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(fileName);
                var node = doc.SelectSingleNode("//" + nodeName);
                Assert.NotNull(node);
                File.Delete(fileName);
            }

            [Test]
            public void AddSectionNode_Existing_AttributeAdded()
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(Properties.Resources.WebConfig);

                var nodeName = doc.SelectSingleNode("//configuration").ChildNodes[0].Name;

                string fileName = "F" + Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(fileName, Properties.Resources.WebConfig);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["ddd"] = "dddfff";
                Installation.AddSectionNode(nodeName, dic, nodeName, fileName, "//configuration");

                doc.Load(fileName);
                var node = doc.SelectSingleNode("//" + nodeName + "/@ddd");
                Assert.NotNull(node);
                Assert.AreEqual("dddfff", node.Value);
                File.Delete(fileName);
            }

            [Test]
            public void AddSectionNode_Existing_AttributeOverwritten()
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(Properties.Resources.WebConfig);

                var nodeName = "dataProviderKeys";

                string fileName = "F" + Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(fileName, Properties.Resources.WebConfig);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["configProtectionProvider"] = "dddfff";
                Installation.AddSectionNode(nodeName, dic, nodeName, fileName, "//dataProvidersGroup");

                doc.Load(fileName);
                var node = doc.SelectSingleNode("//" + nodeName + "/@configProtectionProvider");
                Assert.NotNull(node);
                Assert.AreEqual("dddfff", node.Value);
                File.Delete(fileName);
            }

            [Test]
            public void AddSectionNode_ExistingNameDiffValue_NewNodeAdded()
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(Properties.Resources.WebConfig);

                var nodeName = "add";

                string fileName = "F" + Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(fileName, Properties.Resources.WebConfig);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["type"] = "dddfff";
                Installation.AddSectionNode(nodeName, dic, nodeName, fileName, "//dataProviders");

                doc.Load(fileName);
                var node = doc.SelectSingleNode(string.Format("//dataProviders/add[@type='{0}']", dic["type"]));
                Assert.NotNull(node);
                File.Delete(fileName);
            }

            [Test]
            public void AddSectionNode_AddedTwice_OneExists()
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(Properties.Resources.WebConfig);

                var nodeName = "add";

                string fileName = "F" + Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(fileName, Properties.Resources.WebConfig);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["type"] = "dddfff";
                Installation.AddSectionNode(nodeName, dic, nodeName, fileName, "//dataProviders");
                Installation.AddSectionNode(nodeName, dic, nodeName, fileName, "//dataProviders");

                doc.Load(fileName);
                var nodes = doc.SelectNodes(string.Format("//dataProviders/add[@type='{0}']", dic["type"]));
                Assert.AreEqual(1,nodes.Count);
                File.Delete(fileName);
            }
        }

    }
}