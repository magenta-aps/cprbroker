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
using CprBroker.Providers.E_M;

namespace CprBroker.Tests.E_M
{
    public class Program
    {
        string ConnectionString;
        string ServiceUrl = "http://CprBroker/Services/Part.asmx";
        string ApplicationToken = "fb6e6e79-7505-4667-9736-d500092d844b";

        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            var nowDirectoryString = string.Format("Runs\\{0}\\", DateTime.Now.ToString("yyyy MM dd HH_mm"));
            if (!Directory.Exists(nowDirectoryString))
                Directory.CreateDirectory(nowDirectoryString);

            ConnectionString = string.Join(" ", args);
            string logFileName = nowDirectoryString + "Log.txt";
            string succeededFileName = nowDirectoryString + "Succeeded.txt";
            string failedFileName = nowDirectoryString + "Failed.txt";
            string outDir = nowDirectoryString + "Registrations\\";

            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            var logFileWriter = new StreamWriter(logFileName, true) { AutoFlush = true };
            var succeededFileWriter = new StreamWriter(succeededFileName, true) { AutoFlush = true };
            var failedFileWriter = new StreamWriter(failedFileName, true) { AutoFlush = true };

            Console.SetOut(logFileWriter);


            var effectDate = DateTime.Today;

            bool useWebService = true;
            Func<string, DateTime, object> getter;
            if (useWebService)
            {
                getter = GetPersonFromWebService;
            }
            else
            {
                getter = GetPersonFromDatabase;
            }

            using (succeededFileWriter)
            {
                using (failedFileWriter)
                {
                    using (var dataContext = new E_MDataContext(ConnectionString))
                    {
                        var count = dataContext.Citizens.Count();
                        Console.WriteLine(string.Format("Found <{0}>citizens", count));
                        int processed = 0;
                        foreach (var citizen in dataContext.Citizens.OrderBy(c => c.PNR))
                        {
                            string pnr = Converters.ToCprNumber(citizen.PNR);
                            Console.WriteLine(string.Format("Starting citizen <{0}>", pnr));
                            try
                            {
                                var registration = getter(pnr, effectDate);
                                string registrationXml = CprBroker.Utilities.Strings.SerializeObject(registration);
                                string registrationFileName = string.Format("{0}{1}.xml", outDir, pnr);
                                File.WriteAllText(registrationFileName, registrationXml);
                                succeededFileWriter.WriteLine(pnr);
                                Console.WriteLine("Succeeded !!");
                            }
                            catch (Exception ex)
                            {
                                failedFileWriter.WriteLine(pnr);
                                Console.WriteLine(ex.ToString());
                            }
                            finally
                            {
                                processed++;
                                var percent = (100 * processed / count).ToString();
                                Console.WriteLine(string.Format("Processed <{0}> of <{1}> - <{2}%>", processed, count, percent));
                            }
                        }
                    }
                }
            }

        }

        object GetPersonFromDatabase(string pnr, DateTime effectDate)
        {
            using (var dataContext = new E_MDataContext(ConnectionString))
            {
                var citizen = dataContext.Citizens.Where(cit => cit.PNR == decimal.Parse(pnr)).First();
                var registration = citizen.ToRegistreringType1(effectDate, cpr => Guid.NewGuid());
                return registration;
            }
        }

        object GetPersonFromWebService(string pnr, DateTime effectDate)
        {
            var partService = new Part.Part();
            partService.ApplicationHeaderValue = new CprBroker.Tests.E_M.Part.ApplicationHeader()
            {
                ApplicationToken = ApplicationToken,
                UserToken = ""
            };
            partService.Url = ServiceUrl;

            var uuid = partService.GetUuid(pnr);
            if (uuid.StandardRetur.StatusKode != "200")
            {
                throw new Exception(string.Format("GetUuid failed : {0}: {1}", uuid.StandardRetur.StatusKode, uuid.StandardRetur.FejlbeskedTekst));
            }
            var person = partService.RefreshRead(new CprBroker.Tests.E_M.Part.LaesInputType() { UUID = uuid.UUID });
            if (person.StandardRetur.StatusKode != "200")
            {
                throw new Exception(string.Format("Read failed : {0}: {1}", person.StandardRetur.StatusKode, person.StandardRetur.FejlbeskedTekst));
            }
            return person.LaesResultat.Item;
        }

    }
}
