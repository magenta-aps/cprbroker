using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace SchemaGeneration
{
    public partial class WorkFile
    {
        public WorkFile(string xsdPath)
        {
            XsdFullPath = xsdPath;

            var doc = new XmlDocument();
            doc.Load(XsdFullPath);

            TargetNamespace = GetTargetNamespace(doc);
            DefinedTypeNames = GetDefinedTypeNames(doc);
        }

        public string XsdFullPath { get; private set; }
        public DirectoryInfo DirectoryInfo { get { return new FileInfo(this.XsdFullPath).Directory; } }
        public string XsdLocalName { get { return new FileInfo(XsdFullPath).Name; } }
        public List<TypeDef> Types = new List<TypeDef>();
        public string TargetNamespace { get; private set; }
        public string[] DefinedTypeNames { get; private set; }

        public string CodeFullPath
        {
            get { return XsdFullPath.Replace(".xsd", ".designer.cs"); }
        }
        public string CodeLocalName
        {
            get { return XsdLocalName.Replace(".xsd", ".designer.cs"); }
        }

        private string GetTargetNamespace(XmlDocument doc)
        {
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectSingleNode("//xsd:schema", nsMgr);
            var attr = nodes.Attributes["targetNamespace"];
            if (attr != null)
                return attr.Value;
            else
                return null;
        }

        private string[] GetDefinedTypeNames(XmlDocument doc)
        {
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectNodes("//xsd:complexType[@name]", nsMgr).OfType<XmlElement>()
                .Union(doc.SelectNodes("//xsd:simpleType[@name]", nsMgr).OfType<XmlElement>())
                .ToArray();
            var fileTypes = nodes.Select(nd => nd.Attributes["name"].Value).ToArray();
            return fileTypes;
        }

        public bool TypeDefinedInFile(TypeDef typeDef, string fileNamespace, string[] fileTypes)
        {
            var className = typeDef.Name;
            var withSameClassName = fileTypes.Where(t => className.StartsWith(t) && className.Length <= t.Length + 1).SingleOrDefault();
            if (withSameClassName != null)
            {
                string nsPattern = @"\[System\.Xml\.Serialization\.Xml(Type|Root)Attribute\(.*Namespace=""(?<ns>[^""]+)""";
                var nsMatch = Regex.Match(typeDef.Match.Value, nsPattern);
                var ns = nsMatch.Groups["ns"].Value;
                return ns.Equals(fileNamespace);
            }
            return fileTypes.Contains(className);
        }

        public void WriteCodeFile(FileHeader header, string[] includedNamespaces)
        {
            using (var rd = new StreamWriter(this.CodeFullPath))
            {
                rd.Write(header.HeaderMatch.Value);
                foreach (var typeDef in Types)
                {
                    rd.Write(typeDef.Match.Value);
                }
                rd.Write("}");
            }
        }

        public DirectoryInfo CreateTempDir()
        {
            // Copy files to temp directory

            string tempDir = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7);
            var fileInfo = new FileInfo(this.XsdFullPath);
            var dirPath = fileInfo.Directory.FullName + @"\\" + tempDir + @"\";
            Directory.CreateDirectory(dirPath);

            Func<string, string> mapper = (s) => dirPath + new FileInfo(s).Name;
            foreach (var f in fileInfo.Directory.GetFiles("*.xsd"))
            {
                File.Copy(f.FullName, mapper(f.FullName));
            }
            return new DirectoryInfo(dirPath);
        }


    }

}
