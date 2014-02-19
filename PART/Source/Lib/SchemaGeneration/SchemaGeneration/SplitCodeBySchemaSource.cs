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
        public static void BuildCodeFile(string xsdFilePath, string ns)
        {
            var bytes = GetCodeFileBytes(xsdFilePath, ns);
            File.WriteAllBytes(new WorkFile(xsdFilePath).CodeFullPath, bytes);
        }

        public static byte[] GetCodeFileBytes(string xsdFilePath, string ns)
        {
            var f = new WorkFile(xsdFilePath);
            var tmpDir = f.CreateTempDir();
            var allCode = GenerateCode(tmpDir.FullName, ns);
            SplitByFileSource(allCode, tmpDir.FullName);
            var bytes = File.ReadAllBytes(tmpDir + f.CodeLocalName);
            Directory.Delete(tmpDir.FullName, true);
            return bytes;
        }

        public static string GenerateCode(string partialSchemaDir, string ns)
        {
            using (var dirContext = new TempCurrentDirectoryContext(partialSchemaDir))
            {
                var args = new List<string>();
                args.Add("/classes");
                args.Add("/n:" + ns);

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
        }

        public static void SplitByFileSource(string allCodeFile, string partialSchemaDir)
        {
            var sourceFile = new SourceCodeFile(allCodeFile);
            var generatedFiles = sourceFile.ToWorkFiles(partialSchemaDir);
            foreach (var f in generatedFiles)
            {
                f.WriteCodeFile(sourceFile.HeaderMatch);
            }
        }

    }
}
