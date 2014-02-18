using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SchemaGeneration
{
    class ByRegex
    {
        public static void Run()
        {
            string sourceFile = @"C:\Magenta Workspace\PART\Source\Core\Schemas\OIOXSD\XSD.cs";
            string dir = @"C:\Magenta Workspace\PART\Source\Core\Schemas\OIOXSD\";
            string text = File.ReadAllText(sourceFile);

            // Get header
            string headerPattern = ""
                + @"\A"
                //+ @"(^//.*\r\n)*"
                + @"(^((//.*)|(\s*)|(\s*(namespace|using).*))\r\n)*" 
                + @""
                + @""
                + @"";
            var headerMatch = Regex.Match(text, headerPattern, RegexOptions.Multiline);

            // Get types (classes/enums)
            string typePattern = ""
                + @"(^\s{4}///.+\r\n)*" // XML doc line
                + @"(^\s{4}\[.+\]\r\n)*"
                //+ @"^\s{4}public (partial\s)?(class|enum).+\r\n"
                + @"^\s{4}public (partial\s)?(class|enum)\s+(?<typeName>\w+).+\r\n"
                + @"(^\s{8}.*\r\n)*"
                + @"^\s{4}\}\r\n"
                + @"";

            var typeMatches = Regex.Matches(text, typePattern, RegexOptions.Multiline);

            var files = Directory.GetFiles(dir, "*.xsd");

            foreach (var file in files)
            {
                var fileTypes = ByCompileAndGenerate.GetTypesInFile(file);
                var relevantTypes = typeMatches.OfType<Match>().Where(m => fileTypes.Contains(m.Groups["typeName"].Value)).ToArray();

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
    }
}
