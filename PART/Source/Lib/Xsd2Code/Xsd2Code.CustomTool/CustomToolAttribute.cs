using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xsd2Code.CustomTool
{
    /*
 * Copyright (C) 2006 Chris Stefano
 *       cnjs@mweb.co.za
 */

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomToolAttribute : Attribute
    {

        public string Description { get; set; }

        public string Name { get; set; }
    }
}
