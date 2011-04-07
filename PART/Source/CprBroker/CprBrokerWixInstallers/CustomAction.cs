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
        public static ActionResult TestConnection(Session session)
        {

            DatabaseSetupInfo dbInfo = GetDatabaseSetupInfo(session);
            string message = "";
            if (dbInfo.Validate(ref message))
            {
                session["DB_VALID"] = "True";
            }
            else
            {
                session["DB_VALID"] = message;
            }
            return ActionResult.Success;
        }

        private static DatabaseSetupInfo GetDatabaseSetupInfo(Session session)
        {
            DatabaseSetupInfo ret = new DatabaseSetupInfo();
            ret.ServerName = session["DB_SERVERNAME"];
            ret.DatabaseName = session["DB_DATABASENAME"];

            ret.AdminAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
            ret.AdminAuthenticationInfo.IntegratedSecurity = session["DB_ADMININTEGRATEDSECURITY"] == "SSPI";
            if (!ret.AdminAuthenticationInfo.IntegratedSecurity)
            {
                ret.AdminAuthenticationInfo.UserName = session["DB_ADMINUSERNAME"];
                ret.AdminAuthenticationInfo.Password = session["DB_ADMINPASSWORD"];
            }

            ret.ApplicationAuthenticationSameAsAdmin = !string.IsNullOrEmpty(session["DB_APPSAMEASADMIN"]);
            if (!ret.ApplicationAuthenticationSameAsAdmin)
            {
                ret.ApplicationAuthenticationInfo = new DatabaseSetupInfo.AuthenticationInfo();
                ret.ApplicationAuthenticationInfo.IntegratedSecurity = session["DB_APPINTEGRATEDSECURITY"] == "SSPI";
                if (!ret.ApplicationAuthenticationInfo.IntegratedSecurity)
                {
                    ret.ApplicationAuthenticationInfo.UserName = session["DB_APPUSERNAME"];
                    ret.ApplicationAuthenticationInfo.Password = session["DB_APPPASSWORD"];
                }
            }

            return ret;
        }

        [CustomAction]
        public static ActionResult PopulateWebSites(Session session)
        {
            DirectoryEntry w3svc = new DirectoryEntry(WebInstallationInfo.ServerRoot);
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
