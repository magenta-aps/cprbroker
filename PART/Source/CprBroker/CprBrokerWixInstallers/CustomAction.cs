using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers;
using System.Windows.Forms;

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
                MessageBox.Show(Messages.Succeeded, Messages.Succeeded, MessageBoxButtons.OK, MessageBoxIcon.Information); 
                session["DB_VALID"] = "True";                
            }
            else
            {                
                MessageBox.Show(message, Messages.Unsuccessful, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                session["DB_VALID"] = "False";
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
    }
}
