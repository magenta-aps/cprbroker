/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using CprBroker.Engine.Local;
using System.IO;
using FtpLib;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectExtractDataProvider : IPartReadDataProvider, IExternalDataProvider
    {

        public string GetFtpUrl(string subPath = null)
        {
            string url = FtpAddress;
            url = Strings.EnsureStartString(url, "ftp://", false, StringComparison.CurrentCultureIgnoreCase);

            if (FtpPort.HasValue)
                url += ":" + FtpPort;

            if (!string.IsNullOrEmpty(FtpUser) || !string.IsNullOrEmpty(FtpPassword))
            {
                //Admin.LogFormattedSuccess("Adding credentials");
                url = string.Format("{0}:{1}@{2}", FtpUser, FtpPassword, url);
                //ftpRequest.Credentials = new NetworkCredential(FtpUser, FtpPassword);
            }

            url = Strings.EnsureStartString(url, "ftp://", true, StringComparison.CurrentCultureIgnoreCase);

            url = Utilities.Strings.EnsureEndString(url, "/", false);

            subPath = string.Format("{0}", subPath);
            subPath = Utilities.Strings.EnsureStartString(subPath, "/", false);
            subPath = Utilities.Strings.EnsureEndString(subPath, "/", false);

            if (!string.IsNullOrEmpty(subPath))
            {
                url += "/" + subPath;
            }
            Admin.LogFormattedSuccess("URL <{0}>", url);
            return url;
        }

        public FtpConnection CreateFtpConnection(bool open = false)
        {
            var ret = new FtpConnection(FtpAddress, FtpUser, FtpPassword);
            if (open)
            {
                ret.Open();
                ret.Login();
                ret.SetCurrentDirectory(string.Format("'{0}'",this.FtpUser.Substring(1)));
            }
            return ret;
        }

        public FtpFileInfo[] ListFtpContents(string mask = "")
        {
            using (var request = CreateFtpConnection(true))
            {
                return request.GetFiles(mask);
            }
        }

        public string PutTempFileOnFtp()
        {
            using (var request = CreateFtpConnection(true))
            {
                var filePath = CprBroker.Utilities.Strings.NewUniquePath(this.ExtractsFolder, "txt");
                File.WriteAllText(filePath, "ABC");
                request.PutFile(filePath);
                File.Delete(filePath);
                return new FileInfo(filePath).Name;
            }
        }

        public void DeleteFile(string subPath)
        {
            using (var request = CreateFtpConnection(true))
            {
                request.RemoveFile(subPath);
            }
        }

        public void DownloadFile(string subPath)
        {
            string localFileName = ExtractsFolder + "\\" + subPath;
            using (var request = CreateFtpConnection(true))
            {
                request.GetFile(subPath, localFileName, true);
            }
        }

    }
}
