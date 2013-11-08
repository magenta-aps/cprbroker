using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.SetupCpr
{
    /// <summary>
    /// Contains text messages displayed by the application
    /// </summary>
    public class Messages : CPRBroker.SetupDatabase.Messages
    {
        public static readonly string WebsiteExists = "Website already exists. Do you want to overwrite it?";
        public static readonly string WebAppExists = "Web application already exists. Do you want to overwrite it?";
    }
}
