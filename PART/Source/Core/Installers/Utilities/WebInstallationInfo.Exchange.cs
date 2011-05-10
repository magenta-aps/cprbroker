/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
            if (session.GetMode(InstallRunMode.Scheduled) || session.GetMode(InstallRunMode.Rollback))
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
            if (session.GetMode(InstallRunMode.Scheduled) || session.GetMode(InstallRunMode.Rollback))
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
