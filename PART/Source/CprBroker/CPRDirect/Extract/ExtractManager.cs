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
using System.IO;
using System.Data.SqlClient;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Data.DataProviders;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractManager
    {
        public static void ImportFile(string path)
        {
            var text = File.ReadAllText(path, Constants.DefaultEncoding);
            ImportText(text, path);
        }

        public static void ImportText(string text, string sourceFileName = "")
        {
            var extract = new Extract(text, Constants.DataObjectMap, Constants.ReversibleRelationshipMap, sourceFileName);

            using (var conn = new SqlConnection(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    conn.BulkInsertAll<Extract>(new Extract[] { extract }, trans);
                    conn.BulkInsertAll<ExtractItem>(extract.ExtractItems, trans);
                    trans.Commit();
                }
            }

            extract.RefreshPersons();
        }

        public static IndividualResponseType GetPerson(string pnr)
        {
            using (var dataContext = new ExtractDataContext())
            {
                return Extract.GetPerson(pnr, dataContext.ExtractItems, Constants.DataObjectMap);
            }
        }

        public static void ImportDataProviderFolders()
        {
            Admin.LogSuccess("Loading CPR Direct data providers");

            try
            {
                var dbProv = CprBroker.Engine.DataProviderManager.ReadDatabaseDataProviders();
                var result = CprBroker.Engine.DataProviderManager.LoadExternalDataProviders(dbProv, typeof(CPRDirectExtractDataProvider)).Select(p => p as CPRDirectExtractDataProvider).ToArray();

                Admin.LogFormattedSuccess("Found {0} CPR Direct providers", result.Length);

                foreach (var prov in result)
                {
                    try
                    {
                        Admin.LogFormattedSuccess("Checking folder {0}", prov.ExtractsFolder);
                        if (Directory.Exists(prov.ExtractsFolder))
                        {
                            // Download the files -
                            DownloadFtpFiles(prov);

                            // Now process the files
                            ExtractLocalFiles(prov);
                        }
                        else
                        {
                            Admin.LogFormattedError("Folder <{0}> not found", prov.ExtractsFolder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Admin.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Admin.LogException(ex);
            }
        }

        public static void DownloadFtpFiles(CPRDirectExtractDataProvider prov)
        {
            if (prov.HasFtpSource)
            {
                Admin.LogFormattedSuccess("Listing FTP contents at <{0}> ", prov.FtpAddress);
                var ftpFiles = prov.ListFtpContents();

                foreach (var ftpFile in ftpFiles)
                {
                    try
                    {
                        Admin.LogFormattedSuccess("Downloading FTP file <{0}> ", ftpFile);
                        prov.DownloadFile(ftpFile);
                        Admin.LogFormattedSuccess("Deleting FTP file <{0}> ", ftpFile);
                        prov.DeleteFile(ftpFile);
                    }
                    catch (Exception ex)
                    {
                        Admin.LogException(ex);
                    }
                }
            }
        }

        public static void ExtractLocalFiles(CPRDirectExtractDataProvider prov)
        {
            var files = Directory.GetFiles(prov.ExtractsFolder);
            Admin.LogFormattedSuccess("Found <{0}> files", files.Length);

            foreach (var file in files)
            {
                try
                {
                    Admin.LogFormattedSuccess("Reading file <{0}> ", file);
                    ExtractManager.ImportFile(file);
                    Admin.LogFormattedSuccess("Importing file <{0}> succeeded", file);

                    MoveToProcessed(prov.ExtractsFolder, file);
                    Admin.LogFormattedSuccess("File <{0}> moved to \\Processed folder", file);

                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                }
            }
        }

        public static string MoveToProcessed(string folder, string fileFullPath)
        {
            var processedFolderPath = new DirectoryInfo(folder).FullName + "\\Processed";
            var targetFilePath = processedFolderPath + "\\" + new FileInfo(fileFullPath).Name;

            while (File.Exists(targetFilePath))
            {
                processedFolderPath = new DirectoryInfo(folder).FullName + "\\Processed\\" + Utilities.Strings.NewRandomString(5) + "\\";
                targetFilePath = processedFolderPath + "\\" + new FileInfo(fileFullPath).Name;
            }

            if (!Directory.Exists(processedFolderPath))
            {
                Directory.CreateDirectory(processedFolderPath);
            }

            File.Move(fileFullPath, targetFilePath);
            return targetFilePath;
        }

    }
}
