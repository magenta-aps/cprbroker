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

                var extract = new Extract(text, Constants.DataObjectMap);
                conn.BulkInsertAll<Extract>(new Extract[] { extract });
                conn.BulkInsertAll<ExtractItem>(extract.ExtractItems);
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

            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            var providers = DataProviderManager
                .LoadExternalDataProviders(dbProviders, typeof(CPRDirectDataProvider))
                .Select(p => p as CPRDirectDataProvider)
                .ToArray();

            Admin.LogSuccess(string.Format("Found {0} CPR Direct providers", providers.Length));

            foreach (var dataProvider in providers)
            {
                var folder = dataProvider.ExtractsFolder;
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

                            var processedFolderPath = new DirectoryInfo(folder).FullName + "\\Processed";
                            if (!Directory.Exists(processedFolderPath))
                            {
                                Directory.CreateDirectory(processedFolderPath);
                            }
                            File.Move(file, processedFolderPath + "\\" + new FileInfo(file).Name);
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
    }
}
