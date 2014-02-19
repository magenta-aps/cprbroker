using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemaGeneration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomToolAttribute : Attribute
    {
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
