using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.SetupCpr
{
    /// <summary>
    /// Contains text messages displayed by the application
    /// </summary>
    public class Messages : CprBroker.SetupDatabase.Messages
    {
        public static readonly string WebsiteExists = "Website already exists. Do you want to overwrite it?";
        public static readonly string WebAppExists = "Web application already exists. Do you want to overwrite it?";
        public static readonly string MissingIISComponents = "IIS Metabase and IIS6 Configuration Compatibility are missing from your IIS installation. Please make sure to install all the necessary components and try the installation again";
    }
}
