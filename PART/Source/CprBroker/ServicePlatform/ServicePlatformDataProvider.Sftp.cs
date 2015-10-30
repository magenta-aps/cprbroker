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
                        if (!fileInfo.Name.EndsWith(Constants.MetaDataFilePostfix))
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
                        session.GetFiles(remotePathPrepared, localPathPrepared).Check();
                        session.GetFiles(remotePathPreparedMeta, localPathPreparedMeta).Check();
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

        protected string PrepareRemotePathDir(string subPath)
        {
            String subPathModed = Utilities.Strings.EnsureStartString(subPath, "/", true);
            return Utilities.Strings.EnsureEndString(subPathModed, "/", true);
        }

        protected string PrepareRemotePathFile(string fileName)
        {
            return PrepareRemotePathDir(SftpRemotePath) + Utilities.Strings.EnsureStartString("/", fileName, false);
        }

        protected string PrepareLocalPathFile(string fileName)
        {
            return Utilities.Strings.EnsureEndString(ExtractsFolder, "/", true) + Utilities.Strings.EnsureStartString(fileName, "/", false);
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