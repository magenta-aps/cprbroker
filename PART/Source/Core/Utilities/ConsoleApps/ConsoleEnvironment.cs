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
using System.Threading;

namespace CprBroker.Utilities.ConsoleApps
{
    public class ConsoleEnvironment
    {
        public ConsoleEnvironment()
        {

        }

        public static ConsoleEnvironment Create(string[] args)
        {
            var ret = ParseArguments(args);
            ret.InitializeIO();
            return ret;
        }


        public string PartServiceUrl = "http://CprBroker/Services/Part.asmx";
        public string ApplicationToken = "";
        public string BrokerConnectionString = "";
        public string OtherConnectionString = "";
        public string SourceFile = "";
        public string StartCprNumber = "";
        public string AppToken = "";
        public string UserToken = "";
        public string PersonMasterUrl = "";
        public string PersonMasterSpnName = "";
        public int MaxThreads = 1;



        public string OutDir { get; private set; }
        StreamWriter logFileWriter;
        StreamWriter succeededFileWriter;
        StreamWriter failedFileWriter;
        int count;
        long processed;

        public static ConsoleEnvironment ParseArguments(string[] args)
        {
            var envTypeArg = new CommandArgumentSpec() { Switch = "/envType", ValueRequirement = ValueRequirement.Required, MaxOccurs = 1 };
            var startPnrArg = new CommandArgumentSpec() { Switch = "/startPnr", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var sourceArg = new CommandArgumentSpec() { Switch = "/source", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var partUrlArg = new CommandArgumentSpec() { Switch = "/partUrl", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var brokerArg = new CommandArgumentSpec() { Switch = "/brokerDb", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var otherDbArg = new CommandArgumentSpec() { Switch = "/otherDb", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var appTokenArg = new CommandArgumentSpec() { Switch = "/appToken", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var userTokenArg = new CommandArgumentSpec() { Switch = "/userToken", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var pmUrlArg = new CommandArgumentSpec() { Switch = "/pmUrl", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var pmSpnArg = new CommandArgumentSpec() { Switch = "/pmSpn", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };
            var maxThreadsArg = new CommandArgumentSpec() { Switch = "/maxThreads", ValueRequirement = ValueRequirement.NotRequired, MaxOccurs = 1 };

            var arguments = CommandlineParser.SplitCommandArguments(args);
            CommandlineParser.ValidateCommandline(arguments, new CommandArgumentSpec[] { envTypeArg, startPnrArg, sourceArg, partUrlArg, brokerArg, otherDbArg, appTokenArg, userTokenArg, pmUrlArg, pmSpnArg, maxThreadsArg });

            string envTypeName = envTypeArg.FoundArguments[0].Value;
            var ret = Reflection.CreateInstance<ConsoleEnvironment>(envTypeName);
            if (ret != null)
            {
                ret.PartServiceUrl = partUrlArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.StartCprNumber = startPnrArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.SourceFile = sourceArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.BrokerConnectionString = brokerArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.OtherConnectionString = otherDbArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.ApplicationToken = appTokenArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.UserToken = userTokenArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.PersonMasterUrl = pmUrlArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                ret.PersonMasterSpnName = pmSpnArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                var threads = maxThreadsArg.FoundArguments.Select(a => a.Value).FirstOrDefault();
                var intThreads = 0;
                if (int.TryParse(threads, out intThreads))
                    ret.MaxThreads = intThreads;
                else
                    ret.MaxThreads = 1;
                return ret;
            }
            else
            {
                throw new Exception(string.Format("Invalid environment type <{0}>", envTypeName));
            }
        }

        public void InitializeIO()
        {
            var nowDirectoryString = string.Format("Runs\\{0}\\", DateTime.Now.ToString("yyyy MM dd HH_mm"));
            if (!Directory.Exists(nowDirectoryString))
                Directory.CreateDirectory(nowDirectoryString);
            string logFileName = nowDirectoryString + "Log.txt";
            string succeededFileName = nowDirectoryString + "Succeeded.txt";
            string failedFileName = nowDirectoryString + "Failed.txt";
            OutDir = nowDirectoryString + "Registrations\\";

            if (!Directory.Exists(OutDir))
                Directory.CreateDirectory(OutDir);

            logFileWriter = new StreamWriter(logFileName, true) { AutoFlush = true };
            succeededFileWriter = new StreamWriter(succeededFileName, true) { AutoFlush = true };
            failedFileWriter = new StreamWriter(failedFileName, true) { AutoFlush = true };

        }

        public virtual void Initialize()
        {

        }

        public virtual string[] LoadCprNumbers()
        {
            return new string[0];
        }


        public void Run()
        {
            Initialize();
            var cprNumbers = LoadCprNumbers();
            count = cprNumbers.Count();
            Console.WriteLine(string.Format("Found <{0}> citizens", count));
            processed = 0;

            if (!string.IsNullOrEmpty(StartCprNumber))
            {
                var index = Array.IndexOf<string>(cprNumbers, StartCprNumber);

                if (index != -1)
                {
                    for (int i = 0; i < index; i++)
                    {
                        Log(string.Format("Skipping <{0}>", cprNumbers[i]));
                    }
                    cprNumbers = cprNumbers.Skip(index).ToArray();
                }
                else
                {
                    Log(string.Format("Start PNR <{0}> not found, all persons skipped", StartCprNumber));
                    cprNumbers = new string[] { };
                }
                count = cprNumbers.Length;
                Console.WriteLine(string.Format("Filtered to <{0}> citizens", count));
            }

            var actions = new Action[cprNumbers.Length];

            for (int iCprNumber = 0; iCprNumber < cprNumbers.Length; iCprNumber++)
            {
                var cprNumber = cprNumbers[iCprNumber];
                actions[iCprNumber] = () =>
                    {
                        try
                        {
                            Start(cprNumber);
                            ProcessPerson(cprNumber);
                            Pass(cprNumber);
                        }
                        catch (Exception ex)
                        {
                            Fail(cprNumber, ex.ToString());
                        }
                        finally
                        {
                            End(cprNumber);
                        }
                    };
            }
            if (actions.Length == 1)
            {
                actions[0]();
            }
            else
            {
                var threads = new Thread[MaxThreads];
                long started = -1;

                for (int iThread = 0; iThread < MaxThreads; iThread++)
                {
                    threads[iThread] = new Thread(
                        () =>
                        {
                            while (true)
                            {
                                Interlocked.Increment(ref started);
                                if (started < cprNumbers.Length)
                                {
                                    actions[started]();
                                }
                                else
                                {
                                    break;
                                }
                            }
                        });
                    threads[iThread].Start();
                }
                while (Interlocked.Read(ref processed) < cprNumbers.Length)
                {
                    Thread.Sleep(100);
                }
            }
            EndAll();
        }

        public virtual void ProcessPerson(string pnr)
        {

        }

        public void WriteObject(string cprNumber, object obj)
        {
            lock (this)
            {
                string registrationXml = obj is string ? obj as string : CprBroker.Utilities.Strings.SerializeObject(obj);
                string registrationFileName = string.Format("{0}{1}.xml", OutDir, cprNumber);
                File.WriteAllText(registrationFileName, registrationXml);
            }
        }

        public void Log(string text)
        {
            lock (this)
            {
                Console.WriteLine(text);
                logFileWriter.WriteLine(text);
            }
        }

        public void Start(string cprNumber)
        {
            lock (this)
            {
                Log(string.Format("Starting citizen <{0}> of <{1}>", processed + 1, count));
                Log(string.Format("CPR = <{0}>", cprNumber));
            }
        }

        public void Pass(string cprNumber)
        {
            lock (this)
            {
                succeededFileWriter.WriteLine(cprNumber);
                Log("Succeeded !!");
            }
        }

        public void Fail(string cprNumber, string message)
        {
            lock (this)
            {
                failedFileWriter.WriteLine(cprNumber);
                Log(message);
            }
        }

        public void End(string cprNumber)
        {
            lock (this)
            {
                processed++;
                var percent = (100 * processed / count).ToString();
                Log(string.Format("Processed <{0}> of <{1}> - <{2}%>", processed, count, percent));
            }
        }

        public virtual void EndAll()
        {
            Log("All Done !!");
        }
    }
}
