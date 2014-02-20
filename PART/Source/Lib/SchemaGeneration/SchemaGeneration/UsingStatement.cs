using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SchemaGeneration
{
    public class UsingStatement
    {
        public const string Pattern = ""
            + @"(^\s{0,}using\s(?<ns>[^;]+);\r\n)";

        public string Namespace { get; private set; }

        public UsingStatement(string ns)
        {
            Namespace = ns;
        }

        public UsingStatement(Match m)
            : this(m.Groups["ns"].Value)
        {

        }

        public static UsingStatement[] Parse(string s)
        {
            var matches = Regex.Matches(s, Pattern, RegexOptions.Multiline);
            return matches.OfType<Match>().Select(m => new UsingStatement(m)).ToArray();
        }

        public void Write(TextWriter w)
        {
            w.WriteLine(string.Format("    using {0};", Namespace));
        }

    }
}
