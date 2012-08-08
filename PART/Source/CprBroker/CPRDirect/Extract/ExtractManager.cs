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
            ImportText(text);
        }

        public static void ImportText(string text)
        {
            using (var conn = new SqlConnection(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString))
            {
                conn.Open();

                var extract = new Extract(text, Constants.DataObjectMap, Constants.ReversibleRelationshipMap);
                using (var trans = conn.BeginTransaction())
                {
                    conn.BulkInsertAll<Extract>(new Extract[] { extract }, trans);
                    conn.BulkInsertAll<ExtractItem>(extract.ExtractItems, trans);
                }
            }
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

            // TODO: Handle eading of encrypted config section (dataProviders)

            var result = CprBroker.Engine.Manager.Admin.GetDataProviderList(BrokerContext.Current.UserName, BrokerContext.Current.ApplicationToken);

            if (Schemas.Part.StandardReturType.IsSucceeded(result.StandardRetur))
            {
                var folders = result
                .Item
                .Select(p => p.Attributes.Where(atr => atr.Name == Constants.PropertyNames.ExtractsFolder).FirstOrDefault())
                .Where(attr => attr != null)
                .Select(attr => attr.Value)
                .ToArray();

                Admin.LogSuccess(string.Format("Found {0} CPR Direct providers", folders.Length));

                foreach (var folder in folders)
                {
                    Admin.LogFormattedSuccess("Checking folder {0}", folder);

                    if (Directory.Exists(folder))
                    {
                        var files = Directory.GetFiles(folder);
                        Admin.LogFormattedSuccess("Found <{0}> files", files.Length);

                        foreach (var file in files)
                        {
                            try
                            {
                                Admin.LogFormattedSuccess("Reading file <{0}> ", file);
                                ExtractManager.ImportFile(file);
                                Admin.LogFormattedSuccess("Importing file <{0}> succeeded", file);

                                MoveToProcessed(folder, file);
                                Admin.LogFormattedSuccess("File <{0}> moved to \\Processed folder", file);

                            }
                            catch (Exception ex)
                            {
                                Admin.LogException(ex);
                            }
                        }
                    }
                    else
                    {
                        Admin.LogFormattedError("Folder <{0}> not found", folder);
                    }
                }
            }
            else
            {
                Admin.LogFormattedError("Unable to load data providers: Code={0}; Text={1}", result.StandardRetur.StatusKode, result.StandardRetur.FejlbeskedTekst);
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
