using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Xml;
using System.Reflection;

namespace SchemaGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            SplitCodeBySchemaSource.Run(args[0]);
        }

        

        
    }
}
