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
 * Thomas Kristensen
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
using WinSCP;
using System.IO;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider
    {

        public string[] ListSFtpContents()
        {
            return ListSFtpContents(SftpRemotePath);
        }

        protected string[] ListSFtpContents(string subPath)
        {
            try //TODO: Remove exception handling - logging will be done at a higher level
            {
                List<string> remoteFileNamesInDir = new List<string>();
                using (Session session = new Session())
                {
                    String subPathRemotePrepared = PrepareRemotePathDir(subPath);
                    session.Open(PopulateSftpSessionOptions());
                    RemoteDirectoryInfo directory = session.ListDirectory(subPathRemotePrepared);
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (!fileInfo.Name.EndsWith(Constants.MetaDataFilePostfix) && !fileInfo.IsDirectory)
                        {
                            remoteFileNamesInDir.Add(fileInfo.Name);
                        }
                    }
                }
                return remoteFileNamesInDir.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                throw e;
            }
        }

        public void DeleteSftpFile(string fileName)
        {
            try //TODO: Remove exception handling - logging will be done at a higher level
            {
                using (Session session = new Session())
                {
                    String remotePathPrepared = session.EscapeFileMask(PrepareRemotePathFile(fileName));
                    String remotePathPreparedMeta = session.EscapeFileMask(PrepareRemotePathFile(fileName) + Constants.MetaDataFilePostfix);

                    session.Open(PopulateSftpSessionOptions());
                    if (session.FileExists(remotePathPrepared) && session.FileExists(remotePathPreparedMeta))
                    {
                        session.RemoveFiles(remotePathPrepared).Check();
                        session.RemoveFiles(remotePathPreparedMeta).Check();
                    }
                    else
                    {
                        throw new Exception(String.Format("File {0} or its {1}-file {2} does not exists at remote SFTP-host - unable to delete.", remotePathPrepared, Constants.MetaDataFilePostfix, remotePathPreparedMeta));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                throw e;
            }
        }

        public void DownloadSftpFileToExtractsFolder(String fileName)
        {
            try //TODO: Remove exception handling - logging will be done at a higher level
            {
                String localPathPrepared = PrepareLocalPathFile(fileName);
                String localPathPreparedMeta = localPathPrepared + Constants.MetaDataFilePostfix;

                if (File.Exists(localPathPrepared) || File.Exists(localPathPreparedMeta))
                {
                    throw new Exception(String.Format("File {0} or its {1}-file {2} already exists in local extracts folder. Unable to download.", localPathPrepared, Constants.MetaDataFilePostfix, localPathPreparedMeta));
                }

                using (Session session = new Session())
                {
                    String remotePathPrepared = session.EscapeFileMask(PrepareRemotePathFile(fileName));
                    String remotePathPreparedMeta = session.EscapeFileMask(PrepareRemotePathFile(fileName) + Constants.MetaDataFilePostfix);

                    session.Open(PopulateSftpSessionOptions());
                    if (session.FileExists(remotePathPrepared) && session.FileExists(remotePathPreparedMeta))
                    {
                        session.GetFiles(remotePathPrepared, localPathPrepared, false).Check();
                        session.GetFiles(remotePathPreparedMeta, localPathPreparedMeta, false).Check();
                    }
                    else
                    {
                        throw new Exception(String.Format("File {0} or its {1}-file {2} does not exists at remote SFTP-host - unable to download.", remotePathPrepared, Constants.MetaDataFilePostfix, remotePathPreparedMeta));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                throw e;
            }
        }

        /*
        Please Notice, that the upload function is only provided here for testing purposes - do not use this function in the production environment.
        */
        public void UploadFile(String localUploadDir, String fileName)
        {
            string fullPathToLocalFile = Utilities.Strings.EnsureEndString(localUploadDir, "/", true) + fileName;

            if (!File.Exists(fullPathToLocalFile))
            {
                throw new Exception(String.Format("Local file {0} does not exists. Unable to upload to sftp site.", fullPathToLocalFile));
            }
            using (Session session = new Session())
            {
                String remotePathPrepared = PrepareRemotePathFile(fileName);
                session.Open(PopulateSftpSessionOptions());
                if (!session.FileExists(remotePathPrepared))
                {
                    session.PutFiles(fullPathToLocalFile, remotePathPrepared, false).Check();
                }
                else
                {
                    throw new Exception(String.Format("File {0} already exists at remote SFTP-host - unable to upload.", remotePathPrepared));
                }
            }
        }

        protected string PrepareRemotePathDir(string subPath)
        {
            return Utilities.Strings.EnsureEndString(subPath, "/", true);
        }

        protected string PrepareRemotePathFile(string fileName)
        {
            return PrepareRemotePathDir(SftpRemotePath) + fileName;
        }

        protected string PrepareLocalPathFile(string fileName)
        {
            return Utilities.Strings.EnsureEndString(ExtractsFolder, "\\", true) + fileName;
        }

        protected SessionOptions PopulateSftpSessionOptions()
        {
            return new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = SftpAddress,
                UserName = SftpUser,
                PortNumber = SftpPort,
                SshHostKeyFingerprint = SftpSshHostKeyFingerprint,
                SshPrivateKeyPath = SftpSshPrivateKeyPath,
                SshPrivateKeyPassphrase = SftpSshPrivateKeyPassword
            };
        }




    }
}