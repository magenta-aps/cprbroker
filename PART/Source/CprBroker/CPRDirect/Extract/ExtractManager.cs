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
using System.Data.SqlClient;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Data.DataProviders;
using System.Data;
using CprBroker.Engine.Queues;
using CprBroker.Utilities.Config;
using CprBroker.PartInterface;
using CprBroker.Utilities;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractManager
    {
        public static void ImportText(string text, string sourceFileName = "")
        {
            var parseResult = new ExtractParseSession(text, Constants.DataObjectMap);
            ImportParseResult(parseResult, sourceFileName);
        }

        public static void ImportParseResult(ExtractParseSession parseResult, string sourceFileName, bool enqueueConversion = true)
        {
            var extractId = InitExtract(sourceFileName, parseResult, true);

            using (var conn = new SqlConnection(ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                conn.Open();

                var extractItems = parseResult.ToExtractItems(extractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
                var queueItems = parseResult.ToQueueItems(extractId);
                var extractErrors = parseResult.ToExtractErrors(extractId);

                using (var dataContext = new ExtractDataContext())
                {
                    var extract = dataContext.Extracts.Where(ex => ex.ExtractId == extractId).First();

                    var semaphore = Semaphore.GetById(extract.SemaphoreId.Value);

                    conn.BulkInsertAll<ExtractItem>(extractItems);
                    conn.BulkInsertAll<ExtractError>(extractErrors);

                    if (enqueueConversion)
                    {
                        var stagingQueue = CprBroker.Engine.Queues.Queue.GetQueues<ExtractStagingQueue>(ExtractStagingQueue.QueueId).First();
                        stagingQueue.Enqueue(queueItems, semaphore);
                    }

                    extract.Ready = true;
                    dataContext.SubmitChanges();

                    semaphore.Signal();
                }
            }
        }

        public static void ImportFileInSteps(string path, int batchSize, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Constants.ExtractEncoding;

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
                ExtractParseSession extractSession = new ExtractParseSession();
                var extractId = InitExtract(path, extractSession);
                SkipLines(file, extractId, extractSession, typeMap, batchSize);

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
            using (var dataContext = new ExtractDataContext())
            {
                dataContext.Connection.Open();
                using (dataContext.Transaction = dataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    var extract = dataContext.Extracts.Single(e => e.ExtractId == extractId);
                    var semaphore = Semaphore.GetById(extract.SemaphoreId.Value);


                    // Set start record
                    if (string.IsNullOrEmpty(extract.StartRecord) && extractSession.StartWrapper != null)
                    {
                        extract.StartRecord = extractSession.StartWrapper.Contents;
                        extract.ExtractDate = extractSession.StartWrapper.ProductionDate.Value;
                    }

                    // Staging queue. enqueue BEFORE inserting the extract records, so that we never miss queue events
                    var stagingQueue = Queue.GetQueues<ExtractStagingQueue>().First();
                    stagingQueue.Enqueue(extractSession.ToQueueItems(extract.ExtractId), semaphore);

                    // Extract items and errors
                    var conn = dataContext.Connection as SqlConnection;
                    var trans = dataContext.Transaction as SqlTransaction;
                    conn.BulkInsertAll<ExtractItem>(extractSession.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap), trans);
                    conn.BulkInsertAll<ExtractError>(extractSession.ToExtractErrors(extract.ExtractId), trans);


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
                    dataContext.Transaction.Commit();

                    Admin.LogFormattedSuccess("Batch committed, <{0}> lines, <{1}> total so far", wrappers.Count, extract.ProcessedLines.Value);
                }
            }
        }

        private static Guid InitExtract(string path, ExtractParseSession extractSession, bool forceNew = false)
        {
            Semaphore semaphore;
            Extract extract = null;
            using (var dataContext = new ExtractDataContext())
            {
                // Find existing extract or create a new one
                if (!forceNew)
                {
                    extract = dataContext.Extracts.Where(e => e.Filename == path && e.ProcessedLines != null && !e.Ready).OrderByDescending(e => e.ImportDate).FirstOrDefault();
                }

                if (extract == null)
                {
                    semaphore = Semaphore.Create();
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

        private static void SkipLines(TextReader file, Guid extractId, ExtractParseSession extractSession, Dictionary<string, Type> typeMap, int batchSize)
        {
            using (var dataContext = new ExtractDataContext(CprBroker.Utilities.Config.ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                var extract = dataContext.Extracts.Single(ex => ex.ExtractId == extractId);
                if (extract.ProcessedLines.HasValue)
                {
                    while (extractSession.TotalReadLines < extract.ProcessedLines.Value)
                    {
                        long linesToRead = Math.Min(batchSize, extract.ProcessedLines.Value - extractSession.TotalReadLines);
                        Admin.LogFormattedSuccess("Skipping extract lines: <{0}>", linesToRead);

                        // Read and consume the lines
                        extractSession.MarkNewBatch();
                        var wrappers = CompositeWrapper.Parse(file, typeMap, linesToRead);
                        extractSession.AddLines(wrappers);
                    }
                    Admin.LogFormattedSuccess("Skipped extract lines; total = <{0}>", extractSession.TotalReadLines);
                }
            }
        }

        public static IndividualResponseType GetPerson(string pnr)
        {
            using (var dataContext = new ExtractDataContext())
            {
                return Extract.GetPersonFromLatestExtract(pnr, dataContext.ExtractItems, Constants.DataObjectMap);
            }
        }

        public static void DownloadFtpFiles(IExtractDataProvider prov)
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

                        string tmpDownloadFile = ExtractPaths.TempDownloadFilePath(prov, name, true);
                        Admin.LogFormattedSuccess("Downloading FTP file <{0}> to <{1}>", name, tmpDownloadFile);
                        var len = prov.GetLength(name);
                        prov.DownloadFile(name, tmpDownloadFile, len);

                        var extractFilePath = ExtractPaths.ExtractFilePath(prov, ftpFile);
                        Admin.LogFormattedSuccess("Staging file, moving <{0}> to <{1}>", tmpDownloadFile, extractFilePath);
                        File.Move(tmpDownloadFile, extractFilePath);

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

        public static void ExtractLocalFiles(IExtractDataProvider prov, int batchSize, bool logChecks = false)
        {
            var files = Directory.GetFiles(prov.ExtractsFolder);

            if (logChecks || files.Length > 0)
                Admin.LogFormattedSuccess("Found <{0}> files at <{1}>", files.Length, prov.ExtractsFolder);

            foreach (var file in files)
            {
                try
                {
                    if (prov.IsDataFile(file))
                    {
                        Admin.LogFormattedSuccess("Reading file <{0}> ", file);
                        ExtractManager.ImportFileInSteps(file, batchSize);
                        Admin.LogFormattedSuccess("Importing file <{0}> succeeded", file);
                    }
                    else
                    {
                        // Skip non data files metadata file
                        Admin.LogFormattedSuccess("File <{0}> is not a data file", file);
                    }
                    if (prov.KeepFilesLocally)
                    {
                        var processedFilePath = ExtractPaths.ProcessedFilePath(prov, file, true);
                        File.Move(file, processedFilePath);
                        Admin.LogFormattedSuccess("File <{0}> moved to \\Processed folder <{1}>", file, processedFilePath);
                    }
                    else
                    {
                        File.Delete(file);
                        Admin.LogFormattedSuccess("File <{0}> deleted after processing", file);
                    }
                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                }
            }
        }

        public static void CleanProcessedFolder(IExtractDataProvider prov, bool logChecks = false)
        {
            if (logChecks)
            {
                Admin.LogFormattedSuccess("Checking folder <{0}> for processed extracts", prov.ExtractsFolder);
            }

            if (!prov.KeepFilesLocally)
            {
                var path = ExtractPaths.ProcessedFolder(prov);
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    Admin.LogFormattedSuccess("Deleting extract file <{0}>", file);
                    try
                    {
                        File.Delete(file);
                        Admin.LogFormattedSuccess("Extract file <{0}> deleted successfully", file);
                    }
                    catch (Exception ex)
                    {
                        Admin.LogException(ex, file);
                    }
                }
            }
            else
            {
                if (logChecks)
                {
                    Admin.LogFormattedSuccess("Skipping cleanup for extract folder <{0}>. KeepFilesLocally is true", prov.ExtractsFolder);
                }
            }
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
