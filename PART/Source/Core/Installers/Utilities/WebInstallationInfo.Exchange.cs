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
            if (session.GetMode(InstallRunMode.Scheduled))
            {
                return FromCustomAction(session.CustomActionData);
            }
            else
            {
                WebInstallationInfo ret = new WebInstallationInfo();
                ret.CreateAsWebsite = session["WEB_CREATEASWEBSITE"] == "True";
                ret.ApplicationPath = session["WEB_APPLICATIONPATH"];
                ret.VirtualDirectoryName = session["WEB_VIRTUALDIRECTORYNAME"];
                ret.WebsiteName = session["WEB_SITENAME"];
                ret.WebsitePath = session["WEB_VIRTUALDIRECTORYSITEPATH"];
                ret.InstallDir = session["INSTALLDIR"];
                return ret;
            }
        }

        public void CopyToSession(Session session)
        {
            if (session.GetMode(InstallRunMode.Scheduled))
            {
                this.CopyToCustomActionData(session.CustomActionData);
            }
            else
            {
                session["WEB_CREATEASWEBSITE"] = this.CreateAsWebsite.ToString();
                session["WEB_APPLICATIONPATH"] = this.ApplicationPath;
                session["WEB_VIRTUALDIRECTORYNAME"] = this.VirtualDirectoryName;
                session["WEB_SITENAME"] = this.WebsiteName;
                session["WEB_VIRTUALDIRECTORYSITEPATH"] = this.WebsitePath;
                session["INSTALLDIR"] = this.InstallDir;
            }
        }

        public static WebInstallationInfo FromCustomAction(CustomActionData customActionData)
        {
            WebInstallationInfo ret = new WebInstallationInfo();
            ret.CreateAsWebsite = customActionData["WEB_CREATEASWEBSITE"] == "True";
            ret.ApplicationPath = customActionData["WEB_APPLICATIONPATH"];
            ret.VirtualDirectoryName = customActionData["WEB_VIRTUALDIRECTORYNAME"];
            ret.WebsiteName = customActionData["WEB_SITENAME"];
            ret.WebsitePath = customActionData["WEB_VIRTUALDIRECTORYSITEPATH"];
            ret.InstallDir = customActionData["INSTALLDIR"];
            return ret;
        }

        public void CopyToCustomActionData(CustomActionData customActionData)
        {
            customActionData["WEB_CREATEASWEBSITE"] = this.CreateAsWebsite.ToString();
            customActionData["WEB_APPLICATIONPATH"] = this.ApplicationPath;
            customActionData["WEB_VIRTUALDIRECTORYNAME"] = this.VirtualDirectoryName;
            customActionData["WEB_SITENAME"] = this.WebsiteName;
            customActionData["WEB_VIRTUALDIRECTORYSITEPATH"] = this.WebsitePath;
            customActionData["INSTALLDIR"] = this.InstallDir;
        }


    }
}
