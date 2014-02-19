using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SchemaGeneration
{
    public class FieldDef
    {
        public const string FieldPattern = ""
            + @"^\s{8}private (?<typeName>[^ ]+) (?<fieldName>[^\s]+);";

        public string Name { get; private set; }
        public string TypeName { get; private set; }

        public FieldDef(Match lineMatch)
        {
            Name = lineMatch.Groups["fieldName"].Value;
            TypeName = lineMatch.Groups["typeName"].Value;
        }
    }
}
