using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers;
using System.DirectoryServices;

namespace CprBrokerWixInstallers
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult PopulateWebSites(Session session)
        {
            DirectoryEntry w3svc = new DirectoryEntry(WebInstallationInfo.ServerRoot);

            bool multiWebSiteAllowed = Convert.ToInt32(w3svc.Properties["MaxConnections"].Value) == 0;
            session["WEB_MULTIPLESITESALLOWED"] = (multiWebSiteAllowed).ToString();
            if (!multiWebSiteAllowed)
            {
                session["WEB_CREATEASWEBSITE"] = "False";
            }

            List<DirectoryEntry> websites = new List<DirectoryEntry>();
            foreach (DirectoryEntry de in w3svc.Children)
            {
                if (de.SchemaClassName == "IIsWebServer")
                {
                    websites.Add(de);
                }
            }

            var sitesData = websites
                             .Select(site => new
                             {
                                 Name = site.Properties["ServerComment"].Value as string,
                                 Path = site.Path + "/Root"
                             })
                             .OrderBy(s => s.Name)
                             .ToArray();

            View lView = session.Database.OpenView("SELECT * FROM ComboBox");
            lView.Execute();
            for (int i = 0; i < sitesData.Length; i++)
            {
                var siteData = sitesData[i];
                Record record = session.Database.CreateRecord(4);
                record.SetString(1, "WEB_VIRTUALDIRECTORYSITENAME");
                record.SetInteger(2, i + 1);
                record.SetString(3, siteData.Path);
                record.SetString(4, siteData.Name);

                lView.Modify(ViewModifyMode.InsertTemporary, record);
            }
            if (sitesData.Length > 0)
            {
                session["WEB_VIRTUALDIRECTORYSITENAME"] = sitesData[0].Path;
            }

            return ActionResult.Success;
        }
    }
}
