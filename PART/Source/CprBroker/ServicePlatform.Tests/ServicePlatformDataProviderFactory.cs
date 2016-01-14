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
using System.IO;
using CprBroker.Providers.ServicePlatform;

namespace CprBroker.Tests.ServicePlatform
{
    public class ServicePlatformDataProviderFactory
    {
        static Dictionary<string, string> LoadValues()
        {
            string filePath;
            filePath = @"..\..\prod.dat";
            filePath = @"..\..\demo.dat";
            var lines = File.ReadAllLines(filePath);
            var dic = new Dictionary<string, string>();
            foreach (var l in lines)
            {
                var d = l.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (d.Count() > 0)
                {
                    var key = d.First().TrimStart().TrimEnd();
                    var val = d.Count() > 1 ? d[1].TrimStart().TrimEnd() : "";
                    dic[key.ToLower()] = val;
                }
            }
            return dic;
        }

        public static ServicePlatformDataProvider Create()
        {
            var dic = LoadValues();

            var prov = new CprBroker.Providers.ServicePlatform.ServicePlatformDataProvider()
            {
                ConfigurationProperties = new Dictionary<string, string>(),
                Url = dic["Url".ToLower()],
                UserSystemUUID = dic["UserSystemUUID".ToLower()],
                ServiceAgreementUuid = dic["ServiceAgreementUuid".ToLower()],
                UserUUID = dic["UserUUID".ToLower()],
                CertificateSerialNumber = dic["CertificateSerialNumber".ToLower()],
            };

            return prov;
        }

        public static ServicePlatformExtractDataProvider CreateExtractDataProvider()
        {
            var dic = LoadValues();

            var prov = new CprBroker.Providers.ServicePlatform.ServicePlatformExtractDataProvider()
            {
                ConfigurationProperties = new Dictionary<string, string>(),

                SftpRemotePath = dic["SftpRemotePath".ToLower()],
                //SftpProcessFilesFromSenderName = dic["SftpProcessFilesFromSenderName".ToLower()],
                //SftpRegexFilter = dic["SftpRegexFilter".ToLower()],
                SftpSshHostKeyFingerprint = dic["SftpSshHostKeyFingerprint".ToLower()],
                SftpSshPrivateKeyPassword = dic["SftpSshPrivateKeyPassword".ToLower()],
                SftpSshPrivateKeyPath = dic["SftpSshPrivateKeyPath".ToLower()],
                SftpUser = dic["SftpUser".ToLower()],
                SftpPort = Convert.ToInt32(dic["SftpPort".ToLower()]),
                FtpAddress = dic["SftpAddress".ToLower()],
                ExtractsFolder = dic["ExtractsFolder".ToLower()]
            };

            return prov;
        }
    }
}
