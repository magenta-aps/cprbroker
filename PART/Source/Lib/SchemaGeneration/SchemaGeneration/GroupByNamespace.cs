using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace SchemaGeneration
{
    class GroupByNamespace
    {
        static void Run()
        {
            string dir = @"C:\Magenta Workspace\PART\Source\Core\Schemas\OIOXSD\";
            var files = Directory.GetFiles(dir, "*.xsd");
            var allFileNames = files.Select(f => new FileInfo(f).Name).ToArray();
            var groupedByNamespace = files.Select(f =>
            {
                var text = File.ReadAllText(f);
                var index = text.IndexOf("targetNamespace");
                string ns = "";
                if (index > -1)
                {
                    var index1 = text.IndexOf('"', index);
                    var index2 = text.IndexOf('"', index1 + 1);
                    ns = text.Substring(index1 + 1, index2 - index1 - 1);
                }
                string pat = "element.+name=\"(?<elm>[^\"]+)\"";
                var elelements = System.Text.RegularExpressions.Regex.Matches(text, pat).OfType<System.Text.RegularExpressions.Match>().Select(m => m.Groups["elm"].Captures[0].Value).ToArray();
                return new
                {
                    FileName = new FileInfo(f).Name,
                    TargetNamespace = ns,
                    Elements = elelements
                };
            })
                .GroupBy(t => t.TargetNamespace)
                .Select(g => new
                {
                    Namespace = g.Key,
                    CodeNamespace = string.IsNullOrEmpty(g.Key) ? "NoNs" : g.Key.Replace(":", "_").Replace("/", "_").Replace("\\", "_").Replace(".", "_"),
                    OutputFile = string.IsNullOrEmpty(g.Key) ? "NoNs.cs" : g.Key.Replace(":", "__").Replace("/", "_").Replace("\\", "_").Replace(".", "_") + ".cs",
                    DummySchemaFile = string.IsNullOrEmpty(g.Key) ? "NoNs.xsd" : g.Key.Replace(":", "__").Replace("/", "_").Replace("\\", "_").Replace(".", "_") + ".xsd",
                    InputFiles = g.Select(i => i.FileName).ToArray(),
                    Elements = g.SelectMany(ff => ff.Elements).ToArray()

                })
                .ToArray()
                ;

            var allCommands = groupedByNamespace.Select(schema =>
                string.Format(
                        "Xsd2Code.exe {0} CprBroker.Schemas.Part {1} /eit+",
                        string.Join(" ", schema.InputFiles),
                        schema.OutputFile))
                        .ToArray();
            var command = string.Join(Environment.NewLine, allCommands);
            File.WriteAllLines(@"C:\magenta workspace\part\source\core\schemas\oioxsd\xsd3.txt", allCommands);


            var xsdCommands = groupedByNamespace.Select(schema =>
                string.Format(
                    "copy xsd.xsd {0} /y \r\n" +
                    "xsd.exe /classes /uri:\"{1}\" {2} /n:CprBroker.Schemas.Part.{3} {4} .\\{0} \r\n" +
                    "del {0}",
                        schema.DummySchemaFile,
                        schema.Namespace,
                        string.Join(" ", schema.Elements.Select(e => "/e:" + e)),
                        schema.CodeNamespace,
                        string.Join(" ", allFileNames)
                    ))
                .ToArray();
            File.WriteAllLines(@"C:\magenta workspace\part\source\core\schemas\oioxsd\xsdCommandByNs.txt", xsdCommands);
            /*foreach (var schema in groupedByNamespace)
            {
                File.WriteAllText(dir + schema.DummySchemaFile, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><schema xmlns=\"http://www.w3.org/2001/XMLSchema\">  </schema>");
            }*/

            var allCommands2 = files.Select(f =>
            {
                var inf = new FileInfo(f);
                return string.Format(
                    "Xsd2Code.exe {0} CprBroker.Schemas.Part /eit+",
                    inf.Name
                    );
            })
                .ToArray();
            File.WriteAllLines(@"C:\magenta workspace\part\source\core\schemas\oioxsd\xsd4.txt", allCommands2);
            object o = "";
        }
    }
}
