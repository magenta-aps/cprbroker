using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace SchemaGeneration
{
    class WorkFile
    {
        public WorkFile(string xsdPath)
        {
            XsdFullPath = xsdPath;
        }

        public string XsdFullPath { get; private set; }

        public string CodeFullPath
        {
            get { return XsdFullPath.Replace(".xsd", ".designer.cs"); }
        }

        public List<TypeDef> Types = new List<TypeDef>();

        public string GetTargetNamespace()
        {
            var doc = new XmlDocument();
            doc.Load(XsdFullPath);

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectSingleNode("//xsd:schema", nsMgr);
            var attr = nodes.Attributes["targetNamespace"];
            if (attr != null)
                return attr.Value;
            else
                return null;
        }

        public string[] GetDefinedTypeNames()
        {
            var doc = new XmlDocument();
            doc.Load(XsdFullPath);

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectNodes("//xsd:complexType[@name]", nsMgr).OfType<XmlElement>()
                .Union(doc.SelectNodes("//xsd:simpleType[@name]", nsMgr).OfType<XmlElement>())
                .ToArray();
            var fileTypes = nodes.Select(nd => nd.Attributes["name"].Value).ToArray();
            return fileTypes;
        }

        public bool TypeDefinedInFile(TypeDef typeMatch, string fileNamespace, string[] fileTypes)
        {
            var className = typeMatch.Name;
            var withSameClassName = fileTypes.Where(t => className.StartsWith(t) && className.Length <= t.Length + 1).SingleOrDefault();
            if (withSameClassName != null)
            {
                string nsPattern = @"\[System\.Xml\.Serialization\.Xml(Type|Root)Attribute\(.*Namespace=""(?<ns>[^""]+)""";
                var nsMatch = Regex.Match(typeMatch.Match.Value, nsPattern);
                var ns = nsMatch.Groups["ns"].Value;
                return ns.Equals(fileNamespace);
            }
            return fileTypes.Contains(className);
        }

        public void WriteCodeFile(Match headerMatch, WorkFile file)
        {
            using (var rd = new StreamWriter(file.CodeFullPath))
            {
                rd.Write(headerMatch.Value);
                foreach (var m in Types)
                {
                    rd.Write(m.Match.Value);
                }
                rd.Write("}");
            }
        }
    }

}
