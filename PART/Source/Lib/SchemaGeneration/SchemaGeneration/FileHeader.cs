using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SchemaGeneration
{
    public class FileHeader
    {
        public const string HeaderPattern = ""
                + @"\A"
                + @"(^((//.*)|(\s*)|(\s*(namespace|using).*))\r\n)*";

        public Match HeaderMatch { get; private set; }

        public FileHeader(Match m)
        {
            HeaderMatch = m;
        }
        
    }
}
