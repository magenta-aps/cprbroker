using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SchemaGeneration
{
    class TypeDef
    {
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
