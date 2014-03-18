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

        public static void ImportText(string text, string sourceFileName)
        {
            var parseResult = new ExtractParseResult(text, Constants.DataObjectMap);
            var extract = parseResult.ToExtract(sourceFileName);
            var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
            var extractStaging = parseResult.ToExtractPersonStagings(extract.ExtractId);
            var extractErrors = parseResult.ToExtractErrors(extract.ExtractId);

            using (var conn = new SqlConnection(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    conn.BulkInsertAll<Extract>(new Extract[] { extract }, trans);
                    conn.BulkInsertAll<ExtractItem>(extractItems, trans);
                    conn.BulkInsertAll<ExtractPersonStaging>(extractStaging, trans);
                    conn.BulkInsertAll<ExtractError>(extractErrors, trans);
                    trans.Commit();
                }

                using (var dataContext = new ExtractDataContext())
                {
                    extract = dataContext.Extracts.Where(ex => ex.ExtractId == extract.ExtractId).First();
                    extract.Ready = true;
                    dataContext.SubmitChanges();
                }
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
            var allPnrs = new List<string>();
            using (var file = new StreamReader(dataStream, encoding))
            {
                var extractResult = new ExtractParseResult();

                long totalReadLinesCount = 0;
                using (var conn = new SqlConnection(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString))
                {
                    conn.Open();
                    using (var dataContext = new ExtractDataContext(conn))
                    {
                        // Find existing extract or create a new one
                        var extract = dataContext.Extracts.Where(e => e.Filename == path && e.ProcessedLines != null && !e.Ready).OrderByDescending(e => e.ImportDate).FirstOrDefault();
                        if (extract == null)
                        {
                            extract = extractResult.ToExtract(path, false, 0);
                            Admin.LogFormattedSuccess("Creating new extract <{0}>", extract.ExtractId);
                            dataContext.Extracts.InsertOnSubmit(extract);
                            dataContext.SubmitChanges();
                        }
                        else
                        {
                            Admin.LogFormattedSuccess("Incomplete extract found <{0}>, resuming", extract.ExtractId);
                        }

                        // Start reading the file
                        while (!file.EndOfStream)
                        {
                            var wrappers = CompositeWrapper.Parse(file, Constants.DataObjectMap, batchSize);
                            var batchReadLinesCount = wrappers.Count;
                            totalReadLinesCount += batchReadLinesCount;

                            Admin.LogFormattedSuccess("Batch read, records found <{0}>, total so far <{1}>", batchReadLinesCount, totalReadLinesCount);

                            using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                            {
                                dataContext.Transaction = trans;
                                extractResult.ClearArrays();
                                var uninsertedLinesCount = totalReadLinesCount - extract.ProcessedLines.Value;

                                if (uninsertedLinesCount > 0)
                                {
                                    var linesToSkip = wrappers.Count - (int)uninsertedLinesCount;
                                    if (linesToSkip > 0)
                                    {
                                        Admin.LogFormattedSuccess("Unaligned batch sizes, skipping <{0}> lines", linesToSkip);
                                        wrappers = wrappers.Skip(linesToSkip).ToList();
                                    }

                                    extractResult.AddLines(wrappers);

                                    // Set start record
                                    if (string.IsNullOrEmpty(extract.StartRecord) && extractResult.StartWrapper != null)
                                    {
                                        extract.StartRecord = extractResult.StartWrapper.Contents;
                                        extract.ExtractDate = extractResult.StartWrapper.ProductionDate.Value;
                                    }

                                    // Child records
                                    conn.BulkInsertAll<ExtractItem>(extractResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap), trans);
                                    conn.BulkInsertAll<ExtractError>(extractResult.ToExtractErrors(extract.ExtractId), trans);
                                    // TODO: (Extract) In case some records have been skipped in a previous import attempt, make sure that allPnrs contains their PNR's
                                    conn.BulkInsertAll<ExtractPersonStaging>(extractResult.ToExtractPersonStagings(extract.ExtractId, allPnrs), trans);

                                    // Update counts
                                    extract.ProcessedLines = totalReadLinesCount;

                                    // End record and mark as ready
                                    if (extractResult.EndLine != null)
                                    {
                                        extract.EndRecord = extractResult.EndLine.Contents;
                                        extract.Ready = true;
                                        Admin.LogFormattedSuccess("End record added");
                                    }
                                    dataContext.SubmitChanges();
                                    Admin.LogFormattedSuccess("Batch committed");
                                }
                                else
                                {
                                    Admin.LogFormattedSuccess("Batch already inserted, skipping");
                                }
                                trans.Commit();
                            }
                        }
                    }
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
                        Admin.LogFormattedSuccess("Found FTP file <{0}>", name);
                        name = name.Substring(name.LastIndexOf('D'));

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
                    ExtractManager.ImportFileInSteps(file, CprBroker.Config.Properties.Settings.Default.CprDirectExtractImportBatchSize);
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
                            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - processing PNR <{0}>, person <{1}> of <{2}>", person.ExtractPersonStaging.PNR, personIndex, persons.Length);
                            var uuid = cache.GetUuid(person.ExtractPersonStaging.PNR);
                            var response = Extract.ToIndividualResponseType(person.ExtractPersonStaging.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                            var oioPerson = response.ToRegistreringType1(cache.GetUuid);
                            var personIdentifier = new Schemas.PersonIdentifier() { CprNumber = person.ExtractPersonStaging.PNR, UUID = uuid };
                            UpdateDatabase.UpdatePersonRegistration(personIdentifier, oioPerson);

                            succeeded.Add(person.ExtractPersonStaging.ExtractPersonStagingId);
                            Admin.LogFormattedSuccess("ExtractManager.ConvertPersons() - finished PNR <{0}>, person <{1}> of <{2}>", person.ExtractPersonStaging.PNR, personIndex, persons.Length);
                        }
                        catch (Exception ex)
                        {
                            failed.Add(person.ExtractPersonStaging.ExtractPersonStagingId);
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
