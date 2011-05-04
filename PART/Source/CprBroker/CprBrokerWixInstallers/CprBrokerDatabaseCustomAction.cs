using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers.Installers;
using CprBroker.Installers.CprBrokerInstallers;
using CprBroker.Data.Part;
using CprBroker.Data.Applications;

namespace CprBrokerWixInstallers
{
    public class CprBrokerDatabaseCustomAction
    {
        [CustomAction]
        public static ActionResult TestConnectionString(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session);
        }

        [CustomAction]
        public static ActionResult FinalizeCprBrokerDatabase(Session session)
        {
             List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            ret.Add(new KeyValuePair<string,string>(typeof(CprBroker.Data.Part.AddressCoordinateQualityType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.AddressCoordinateQualityType));
            ret.Add(new KeyValuePair<string, string>(typeof(CprBroker.Data.Applications.Application).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.Application));
            ret.Add(new KeyValuePair<string, string>(typeof(CivilStatusCodeType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.CivilStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(ContactChannelType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.ContactChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(CountrySchemeType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.CountrySchemeType));
            ret.Add(new KeyValuePair<string, string>(typeof(Gender).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.Gender));
            ret.Add(new KeyValuePair<string, string>(typeof(LifecycleStatus).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.LifecycleStatus));
            ret.Add(new KeyValuePair<string, string>(typeof(LifeStatusCodeType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.LifeStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(LogType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.LogType));
            ret.Add(new KeyValuePair<string, string>(typeof(RelationshipType).Name, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.RelationshipType));

            return DatabaseCustomAction.FinalizeDatabase(session, CprBroker.Installers.CprBrokerInstallers.Properties.Resources.CreatePartDatabaseObjects, ret.ToArray());
        }

        [CustomAction]
        public static ActionResult RemoveCprBrokerDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveCprBrokerDatabase(session);
        }
    }
}
