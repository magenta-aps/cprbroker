using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SchemaGeneration
{
    public class FileHeader
    {
        public const string HeaderPattern = ""
                + @"\A"
                + @"(^((//.*)|(\s*)|(\s*(namespace).*))\r\n)*"
                + @"(?<usings>" + UsingStatement.Pattern + "*)"
                + @"(^\s*\r\n)*"
                + @"";

        public Match HeaderMatch { get; private set; }
        public string PreUsings { get; private set; }
        public List<UsingStatement> Usings { get; private set; }
        public string PostUsings { get; private set; }

        public FileHeader(Match m)
        {
            HeaderMatch = m;
            var usings = HeaderMatch.Groups["usings"];
            PreUsings = HeaderMatch.Value.Substring(0, usings.Index);
            Usings = new List<UsingStatement>(UsingStatement.Parse(usings.Value));
            PostUsings = HeaderMatch.Value.Substring(usings.Index + usings.Length);
        }

        public void Write(TextWriter w, string[] includedNamespaces)
        {
            w.Write(PreUsings);
            foreach (var u in Usings.Union(includedNamespaces.Select(s=>new UsingStatement(s))))
            {
                u.Write(w);
            }
            w.Write(PostUsings);
        }
    }
}
