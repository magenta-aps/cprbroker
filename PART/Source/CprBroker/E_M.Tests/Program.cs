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
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            var nowDirectoryString = string.Format("Runs\\{0}\\", DateTime.Now.ToString("yyyy MM dd hh_mm"));
            if (!Directory.Exists(nowDirectoryString))
                Directory.CreateDirectory(nowDirectoryString);

            string connectionString = string.Join(" ", args);
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

            using (succeededFileWriter)
            {
                using (failedFileWriter)
                {
                    using (var dataContext = new E_MDataContext(connectionString))
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
                                var registration = citizen.ToRegistreringType1(effectDate, cpr => Guid.NewGuid());
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

    }
}
