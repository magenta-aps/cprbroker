using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml;

namespace SchemaGeneration
{
    class SplitCodeBySchemaSource
    {
        public static void Run(string partialSchemaDir)
        {
            if (!partialSchemaDir.EndsWith("\\"))
                partialSchemaDir += "\\";

            var allCodeFileName = GenerateCode(partialSchemaDir);

            SplitByFileSource(allCodeFileName, partialSchemaDir);

            File.Delete(allCodeFileName);
            File.Delete(allCodeFileName.Replace(".cs", ".xsd"));
            File.Delete(allCodeFileName.Replace(".cs", ".designer.cs"));
        }

        public static string GenerateCode(string partialSchemaDir)
        {
            Environment.CurrentDirectory = partialSchemaDir;

            var args = new List<string>();
            args.Add("/classes");
            args.Add("/n:CprBroker.Schemas.Part");

            foreach (var f in Directory.GetFiles(partialSchemaDir, "*.xsd"))
            {
                args.Add(string.Format("{0}", new FileInfo(f).Name));
            }

            var tmpFile = partialSchemaDir + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7) + ".xsd";
            using (var w = new StreamWriter(tmpFile))
            {
                w.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><schema xmlns=\"http://www.w3.org/2001/XMLSchema\"></schema>");
            }
            args.Add(string.Format(".\\{0}", new FileInfo(tmpFile).Name));

            var all = string.Join(" ", args.ToArray());
            Console.WriteLine(all);
            var ret = XsdTool.Xsd.Main(args.ToArray());

            if (ret != 0)
            {
                throw new Exception("XSD.exe failed");
            }
            return tmpFile.Replace(".xsd", ".cs");
        }

        public static void SplitByFileSource(string allCodeFile, string partialSchemaDir)
        {

            string text = File.ReadAllText(allCodeFile);

            // Get header
            string headerPattern = ""
                + @"\A"
                + @"(^((//.*)|(\s*)|(\s*(namespace|using).*))\r\n)*"
                + @""
                + @""
                + @"";
            var headerMatch = Regex.Match(text, headerPattern, RegexOptions.Multiline);

            // Get types (classes/enums)
            string typePattern = ""
                + @"(^\s{4}///.+\r\n)*" // XML doc line
                + @"(^\s{4}\[.+\]\r\n)*"
                + @"^\s{4}public ((abstract|partial)\s)*(class|enum)\s+(?<typeName>\w+).+\r\n"
                + @"(^\s{8}.*\r\n)*"
                + @"^\s{4}\}\r\n"
                + @"";

            var typeMatches = Regex.Matches(text, typePattern, RegexOptions.Multiline).OfType<Match>().ToArray();

            var files = Directory.GetFiles(partialSchemaDir, "*.xsd");

            foreach (var file in files)
            {
                var fileTypes = ByCompileAndGenerate.GetTypesInFile(file);
                var fileNamespace = GetNamespace(file);

                var relevantTypes = typeMatches.Where(
                    m =>
                    {
                        var className = m.Groups["typeName"].Value;
                        var withSameClassName = fileTypes.Where(t => className.StartsWith(t) && className.Length <= t.Length + 1).SingleOrDefault();
                        if (withSameClassName != null)
                        {
                            string nsPattern = @"\[System\.Xml\.Serialization\.Xml(Type|Root)Attribute\(.*Namespace=""(?<ns>[^""]+)""";
                            var nsMatch = Regex.Match(m.Value, nsPattern);
                            var ns = nsMatch.Groups["ns"].Value;
                            return ns.Equals(fileNamespace);
                        }
                        return fileTypes.Contains(className);
                    }
                    ).ToArray();

                string codeFileName = file.Replace(".xsd", ".designer.cs");
                using (var rd = new StreamWriter(codeFileName))
                {
                    rd.Write(headerMatch.Value);
                    foreach (var m in relevantTypes)
                    {
                        rd.Write(m.Value);
                    }
                    rd.Write("}");
                }

            }
        }

        public static string GetNamespace(string file)
        {
            var doc = new XmlDocument();
            doc.Load(file);

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var nodes = doc.SelectSingleNode("//xsd:schema", nsMgr);
            var attr = nodes.Attributes["targetNamespace"];
            if (attr != null)
                return attr.Value;
            else
                return null;

        }
    }
}
