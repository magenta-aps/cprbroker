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
    public abstract partial class WebInstallationInfo
    {

        public static WebInstallationInfo FromSession(Session session)
        {
            if (session.GetMode(InstallRunMode.Scheduled) || session.GetMode(InstallRunMode.Rollback))
            {
                return FromObject((key) => session.CustomActionData[key]);
            }
            else
            {
                return FromObject((key) => session[key]);
            }
        }

        private static WebInstallationInfo FromObject(Func<string, string> propGetter)
        {
            bool createAsWebsite = propGetter("WEB_CREATEASWEBSITE") == "True";
            if (createAsWebsite)
            {
                return new WebsiteInstallationInfo()
                {
                    //CreateAsWebsite = propGetter("WEB_CREATEASWEBSITE") == "True",
                    //ApplicationPath = propGetter("WEB_APPLICATIONPATH"),
                    //VirtualDirectoryName = propGetter("WEB_VIRTUALDIRECTORYNAME"),
                    WebsiteName = propGetter("WEB_SITENAME"),
                    //WebsitePath = propGetter("WEB_VIRTUALDIRECTORYSITEPATH"),
                    InstallDir = propGetter("INSTALLDIR"),
                };
            }
            else
            {
                return new VirtualDirectoryInstallationInfo()
                {
                    //CreateAsWebsite = propGetter("WEB_CREATEASWEBSITE") == "True",
                    //ApplicationPath = propGetter("WEB_APPLICATIONPATH"),
                    VirtualDirectoryName = propGetter("WEB_VIRTUALDIRECTORYNAME"),
                    WebsiteName = propGetter("WEB_SITENAME"),
                    //WebsitePath = propGetter("WEB_VIRTUALDIRECTORYSITEPATH"),
                    InstallDir = propGetter("INSTALLDIR"),
                };
            }
        }

        public void CopyToSession(Session session)
        {
            if (session.GetMode(InstallRunMode.Scheduled) || session.GetMode(InstallRunMode.Rollback))
            {
                this.CopyToObject((key, value) => session.CustomActionData[key] = value);
            }
            else
            {
                this.CopyToObject((key, value) => session[key] = value);
            }
        }

        public void CopyToObject(Action<string, string> propSetter)
        {
            if (this is WebsiteInstallationInfo)
            {
                WebsiteInstallationInfo websiteInstallationInfo = this as WebsiteInstallationInfo;
                propSetter("WEB_CREATEASWEBSITE", websiteInstallationInfo.CreateAsWebsite.ToString());
                propSetter("WEB_APPLICATIONPATH", websiteInstallationInfo.TargetWmiPath);
                //propSetter("WEB_VIRTUALDIRECTORYNAME", websiteInstallationInfo.VirtualDirectoryName);
                propSetter("WEB_SITENAME", websiteInstallationInfo.WebsiteName);
                //propSetter("WEB_VIRTUALDIRECTORYSITEPATH", websiteInstallationInfo.WebsitePath);
                propSetter("INSTALLDIR", websiteInstallationInfo.InstallDir);
            }
            else
            {
                VirtualDirectoryInstallationInfo virtualDirectoryInstallationInfo = this as VirtualDirectoryInstallationInfo;
                propSetter("WEB_CREATEASWEBSITE", virtualDirectoryInstallationInfo.CreateAsWebsite.ToString());
                propSetter("WEB_APPLICATIONPATH", virtualDirectoryInstallationInfo.TargetWmiPath);
                propSetter("WEB_VIRTUALDIRECTORYNAME", virtualDirectoryInstallationInfo.VirtualDirectoryName);
                propSetter("WEB_SITENAME", virtualDirectoryInstallationInfo.WebsiteName);
                propSetter("WEB_VIRTUALDIRECTORYSITEPATH", virtualDirectoryInstallationInfo.WebsitePath);
                propSetter("INSTALLDIR", virtualDirectoryInstallationInfo.InstallDir);
            }
        }

    }
}
