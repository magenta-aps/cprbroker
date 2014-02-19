using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SchemaGeneration
{
    public class TypeDef
    {
        public const string TypePattern = ""
                + @"(^\s{4}///.+\r\n)*" // XML doc line
                + @"(^\s{4}\[.+\]\r\n)*"
                + @"^\s{4}public ((abstract|partial)\s)*(class|enum)\s+(?<typeName>\w+).+\r\n"
                + @"(^\s{8}.*\r\n)*"
                + @"^\s{4}\}\r\n"
                + @"";
            
        private TypeDef()
        { }

        public TypeDef(Match m)
        {
            Match = m;
        }

        public Match Match;

        public string Name
        {
            get { return Match.Groups["typeName"].Value; }
        }
    }
}
