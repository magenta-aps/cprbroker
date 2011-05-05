using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CprBroker.Installers
{
    public partial class WebInstallationInfo
    {
        public static WebInstallationInfo FromSession(Session session)
        {
            WebInstallationInfo ret = new WebInstallationInfo();
            ret.CreateAsWebsite = session["WEB_CREATEASWEBSITE"] == "True";
            ret.ApplicationPath = session["WEB_APPLICATIONPATH"];
            ret.ApplicationInstalled = session["WEB_APPLICATIONINSTALLED"] == "True";
            ret.VirtualDirectoryName = session["WEB_VIRTUALDIRECTORYNAME"];
            ret.WebsiteName = session["WEB_SITENAME"];
            ret.WebsitePath = session["WEB_VIRTUALDIRECTORYSITEPATH"];

            return ret;
        }

        public void CopyToSession(Session session)
        {
            session["WEB_CREATEASWEBSITE"] = this.CreateAsWebsite.ToString();
            session["WEB_APPLICATIONPATH"] = this.ApplicationPath;
            session["WEB_APPLICATIONINSTALLED"] = this.ApplicationInstalled.ToString();
            session["WEB_VIRTUALDIRECTORYNAME"] = this.VirtualDirectoryName;
            session["WEB_SITENAME"] = this.WebsiteName;
            session["WEB_VIRTUALDIRECTORYSITEPATH"] = this.WebsitePath;
        }
    }
}
