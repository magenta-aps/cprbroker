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
using CprBroker.Engine.Part;
using CprBroker.Data.DataProviders;
using System.Transactions;
using CprBroker.Engine.Queues;
using CprBroker.Utilities.Config;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractManager
    {
        public static void ImportFile(string path)
        {
            var text = File.ReadAllText(path, Constants.ExtractEncoding);
            ImportText(text, path);
        }

        public static void ImportText(string text)
        {
            ImportText(text, "");
        }

        public static TransactionScope CreateTransactionScope()
        {
            return new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = System.Transactions.TransactionManager.MaximumTimeout
                });
        }

        public static void ImportText(string text, string sourceFileName)
        {
            var parseResult = new ExtractParseSession(text, Constants.DataObjectMap);
            var extract = parseResult.ToExtract(sourceFileName);
            var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);            
            var queueItems = parseResult.ToQueueItems(extract.ExtractId);
            var extractErrors = parseResult.ToExtractErrors(extract.ExtractId);

            using (var transactionScope = CreateTransactionScope())
            {
                using (var conn = new SqlConnection(ConfigManager.Current.Settings.CprBrokerConnectionString))
                {
                    conn.Open();

                    using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                    {
                        conn.BulkInsertAll<Extract>(new Extract[] { extract });
                        conn.BulkInsertAll<ExtractItem>(extractItems);
                        conn.BulkInsertAll<ExtractError>(extractErrors);
                        trans.Commit();
                    }


                    var stagingQueue = CprBroker.Engine.Queues.Queue.GetQueues<ExtractStagingQueue>(ExtractStagingQueue.QueueId).First();
                    stagingQueue.Enqueue(queueItems);

                    using (var dataContext = new ExtractDataContext())
                    {
                        extract = dataContext.Extracts.Where(ex => ex.ExtractId == extract.ExtractId).First();
                        extract.Ready = true;
                        dataContext.SubmitChanges();
                    }
                }
                transactionScope.Complete();
            }
        }

        public static void ImportFileInSteps(string path, int batchSize)
        {
            ImportFileInSteps(path, batchSize, Constants.ExtractEncoding);
        }

        public static void ImportFileInSteps(string path, int batchSize, Encoding encoding)
        {
            Admin.LogFormattedSuccess("Importing file <{0}>, batch size <{1}>, encoding <{2}>", path, batchSize, encoding.EncodingName);
            using (var dataStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                ImportFileInSteps(dataStream, path, batchSize, encoding);
            }
        }

        public static void ImportFileInSteps(Stream dataStream, string path, int batchSize, Encoding encoding)
        {
            var typeMap = Constants.DataObjectMap;
            using (var file = new StreamReader(dataStream, encoding))
            {
                // Init extract and skip already read lines
                var extractId = InitExtract(path);
                SkipLines(file, extractId, typeMap);

                ExtractParseSession extractSession = new ExtractParseSession();
                while (!file.EndOfStream)
                {
                    extractSession.MarkNewBatch();
                    ImportOneBatch(file, extractId, extractSession, typeMap, batchSize);
                }
            }
        }

        private static void ImportOneBatch(StreamReader file, Guid extractId, ExtractParseSession extractSession, Dictionary<string, Type> typeMap, int batchSize)
        {
            // Start reading the file
            var wrappers = CompositeWrapper.Parse(file, typeMap, batchSize);
            extractSession.AddLines(wrappers);

            // Database access
            using (var conn = new SqlConnection(ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                conn.Open();
                using (var dataContext = new ExtractDataContext(conn))
                {
                    var extract = dataContext.Extracts.Single(e => e.ExtractId == extractId);
                    var semaphore = Semaphore.GetById(extract.SemaphoreId.Value);

                    // Init transaction block
                    using (var transactionScope = CreateTransactionScope())
                    {
                        // Set start record
                        if (string.IsNullOrEmpty(extract.StartRecord) && extractSession.StartWrapper != null)
                        {
                            extract.StartRecord = extractSession.StartWrapper.Contents;
                            extract.ExtractDate = extractSession.StartWrapper.ProductionDate.Value;
                        }

                        // Extract items and errors
                        conn.BulkInsertAll<ExtractItem>(extractSession.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap));
                        conn.BulkInsertAll<ExtractError>(extractSession.ToExtractErrors(extract.ExtractId));

                        // Staging queue
                        var stagingQueue = Queue.GetQueues<ExtractStagingQueue>().First();
                        stagingQueue.Enqueue(extractSession.ToQueueItems(extract.ExtractId), semaphore);

                        // Update counts and commit
                        extract.ProcessedLines += wrappers.Count;

                        // End record and mark as ready, and signal the semaphore
                        if (extractSession.EndLine != null)
                        {
                            extract.EndRecord = extractSession.EndLine.Contents;
                            extract.Ready = true;
                            semaphore.Signal();
                        }

                        // Commit
                        dataContext.SubmitChanges();
                        transactionScope.Complete();
                    }
                    Admin.LogFormattedSuccess("Batch committed, <{0}> lines, <{1}> total so far", wrappers.Count, extract.ProcessedLines.Value);
                }
            }
        }

        private static Guid InitExtract(string path)
        {
            Semaphore semaphore;
            Extract extract;
            using (var dataContext = new ExtractDataContext(CprBroker.Utilities.Config.ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                // Find existing extract or create a new one
                extract = dataContext.Extracts.Where(e => e.Filename == path && e.ProcessedLines != null && !e.Ready).OrderByDescending(e => e.ImportDate).FirstOrDefault();

                if (extract == null)
                {
                    semaphore = Semaphore.Create();
                    var extractSession = new ExtractParseSession();
                    extract = extractSession.ToExtract(path, false, 0, semaphore);
                    Admin.LogFormattedSuccess("Creating new extract <{0}>", extract.ExtractId);
                    dataContext.Extracts.InsertOnSubmit(extract);
                    dataContext.SubmitChanges();
                }
                else
                {
                    if (extract.SemaphoreId.HasValue)
                    {
                        semaphore = Semaphore.GetById(extract.SemaphoreId.Value);
                    }
                    else
                    {
                        // Transitional logic for the rare case of an extract that has started before introduction of semaphores
                        semaphore = Semaphore.Create();
                        extract.SemaphoreId = semaphore.Impl.SemaphoreId;
                        dataContext.SubmitChanges();
                    }
                    Admin.LogFormattedSuccess("Incomplete extract found <{0}>, resuming", extract.ExtractId);
                }
                return extract.ExtractId;
            }
        }

        private static void SkipLines(TextReader file, Guid extractId, Dictionary<string, Type> typeMap)
        {
            using (var dataContext = new ExtractDataContext(CprBroker.Utilities.Config.ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                var extract = dataContext.Extracts.Single(ex => ex.ExtractId == extractId);
                if (extract.ProcessedLines.HasValue)
                    CompositeWrapper.Skip(file, typeMap, extract.ProcessedLines.Value);
            }
        }

        public static IndividualResponseType GetPerson(string pnr)
        {
            using (var dataContext = new ExtractDataContext())
            {
                return Extract.GetPersonFromLatestExtract(pnr, dataContext.ExtractItems, Constants.DataObjectMap);
            }
        }

        public static void DownloadFtpFiles(CPRDirectExtractDataProvider prov)
        {
            if (prov.HasFtpSource)
            {
                Admin.LogFormattedSuccess("Listing FTP contents at <{0}> ", prov.FtpAddress);
                var ftpFiles = prov.ListFtpContents();
                Admin.LogFormattedSuccess("Found <{0}> files at FTP <{1}> ", ftpFiles.Count(), prov.FtpAddress);
                foreach (var ftpFile in ftpFiles)
                {
                    try
                    {
                        string name = ftpFile;
                        Admin.LogFormattedSuccess("Downloading FTP file <{0}>", name);
                        var len = prov.GetLength(name);
                        prov.DownloadFile(name, len);
                        Admin.LogFormattedSuccess("Deleting FTP file <{0}> ", ftpFile);
                        prov.DeleteFile(name);
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
                    ExtractManager.ImportFileInSteps(file, CprBroker.Utilities.Config.ConfigManager.Current.Settings.CprDirectExtractImportBatchSize);
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

        public static void ConvertPersons()
        {
            ConvertPersons(1);
        }

        public static void ConvertPersons(int batchSize)
        {
            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() started, batch size <{0}>", batchSize);
            List<Guid> succeeded = new List<Guid>(), failed = new List<Guid>();

            using (var dataContext = new ExtractDataContext())
            {
                var dataLoadOptions = new System.Data.Linq.DataLoadOptions();
                dataLoadOptions.LoadWith<ExtractItem>(ei => ei.Extract);
                dataContext.LoadOptions = dataLoadOptions;

                var persons = ExtractPersonStaging.SelectTop(dataContext, batchSize);
                Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - <{0}> persons found", persons.Length);

                var conversionExtracts = ExtractConversion.CreateFromPersonStagings(persons);
                ExtractConversion.FillExtractItems(conversionExtracts, dataContext);

                // Preload UUIDs
                var pnrs = ExtractConversion.AllPNRs(conversionExtracts);
                var cache = new UuidCache();
                cache.FillCache(pnrs);

                int personIndex = 0;
                foreach (var personGroup in conversionExtracts)
                {
                    Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - converting persons from extract <{0}>", personGroup.Extract.ExtractId);
                    foreach (var person in personGroup.Persons)
                    {
                        personIndex++;
                        try
                        {
                            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - processing PNR <{0}>, person <{1}> of <{2}>", person.PNR, personIndex, persons.Length);
                            var uuid = cache.GetUuid(person.PNR);
                            var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                            var oioPerson = response.ToRegistreringType1(cache.GetUuid);
                            var personIdentifier = new Schemas.PersonIdentifier() { CprNumber = person.PNR, UUID = uuid };
                            UpdateDatabase.UpdatePersonRegistration(personIdentifier, oioPerson);

                            succeeded.Add(person.ExtractPersonStagingId);
                            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - finished PNR <{0}>, person <{1}> of <{2}>", person.PNR, personIndex, persons.Length);
                        }
                        catch (Exception ex)
                        {
                            failed.Add(person.ExtractPersonStagingId);
                            Admin.LogException(ex);
                        }
                    }
                }
            }

            // Delete the staging tables from a new data context to maximize performance
            using (var dataContext = new ExtractDataContext())
            {
                var persons = dataContext.ExtractPersonStagings.Where(ep => succeeded.Contains(ep.ExtractPersonStagingId));
                dataContext.ExtractPersonStagings.DeleteAllOnSubmit(persons);
                dataContext.SubmitChanges();
            }
            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() ending, batch size: <{0}>, succeeded: <{1}>, failed: <{2}>", batchSize, succeeded.Count, failed.Count);
        }

    }
}
