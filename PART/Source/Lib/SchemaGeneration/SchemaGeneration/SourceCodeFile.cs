using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SchemaGeneration
{
    public class SourceCodeFile
    {

        public const string HeaderPattern = ""
                + @"\A"
                + @"(^((//.*)|(\s*)|(\s*(namespace|using).*))\r\n)*";

        public Match HeaderMatch { get; private set; }

        public TypeDef[] Types { get; private set; }

        public SourceCodeFile(string path)
        {
            var text = File.ReadAllText(path);
            HeaderMatch = Regex.Match(text, HeaderPattern, RegexOptions.Multiline);

            Types = Regex.Matches(text, TypeDef.TypePattern, RegexOptions.Multiline)
                .OfType<Match>()
                .Select(m => new TypeDef(m))
                .ToArray();
        }
    }
}
