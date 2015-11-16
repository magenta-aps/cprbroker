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

namespace CprBroker.Providers.ServicePlatform
{
    public class Constants
    {
        public static readonly Guid ActorId = new Guid("{9FD5B2AA-1C83-4FFE-A036-E38308B83A98}");
        public static readonly string MetaDataFilePostfix = ".metadata";
        public class ConfigProperties
        {
            public const string Url = "Url";
            public const string ServiceAgreementUuid = "Service agreement UUID";
            public const string UserUUID = "User UUID";
            public const string UserSystemUUID = "User system UUID";
            public const string CertificateSerialNumber = "Certificate serial number";
            public static readonly string HasSftpSource = "Has SFTP Source";
            public static readonly string SftpAddress = "SFTP Address";
            public static readonly string SftpPort = "SFTP Port";
            public static readonly string ExtractsFolder = "Extracts folder";
            public static readonly string SftpUser = "SFTP User";
            public static readonly string SftpRegexFilter = "Filename Regex filter";
            public static readonly string SftpSshPrivateKeyPath = "SFTP SSH Private Key Path";
            public static readonly string SftpSshPrivateKeyPassword = "SFTP SSH Private Key Password";
            public static readonly string SftpSshHostKeyFingerprint = "SFTP SSH Host Key Fingerprint";
            public static readonly string SftpProcessFilesFromSenderName = "SFTP Process Files From Sender Named";
            public static readonly string SftpRemotePath = "SFTP Remote Path (e.g. \\IN)";

        }
    }
}
