using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchClient
{
    class LogFileAnalyzer
    {
        public static void AnalyzeLogFile(params string[] args)
        {
            string logfilePath = args[0];
            long calls = 0;
            long length = 0;
            long lines = 0;
            var dateCallCounts = new Dictionary<DateTime, int>();
            var appCallCounts = new Dictionary<string, int>();
            var pnrCallCounts = new Dictionary<string, int>();

            bool called = false;
            DateTime date = DateTime.MinValue;
            string app = "";
            string pnr = "";

            using (var rd = new System.IO.StreamReader(logfilePath))
            {
                while (!rd.EndOfStream)
                {
                    string s = rd.ReadLine();
                    lines++;
                    length += s.Length;

                    if (s.StartsWith("Timestamp: "))
                    {
                        date = DateTime.Parse(s.Substring(11)).Date;
                    }
                    else if (s.StartsWith("Message: Calling AS7820"))
                    {
                        called = true;
                        calls++;
                        pnr = s.Substring(26, 10);
                    }
                    else if (s.StartsWith("ApplicationName - "))
                    {
                        app = s.Substring(18);
                    }
                    else if (s.StartsWith("----------------------------------------"))
                    {
                        if (!appCallCounts.ContainsKey(app))
                        {
                            appCallCounts[app] = 0;
                        }
                        if (!dateCallCounts.ContainsKey(date))
                        {
                            dateCallCounts[date] = 0;
                        }
                        if (!pnrCallCounts.ContainsKey(pnr))
                        {
                            pnrCallCounts[pnr] = 0;
                        }
                        if (called)
                        {
                            appCallCounts[app]++;
                            dateCallCounts[date]++;
                            pnrCallCounts[pnr]++;
                        }

                        called = false;
                        app = "";
                    }

                    if (lines % 10000 == 0)
                    {
                        Console.WriteLine("Calls so far <{0}>; lines so far <{1}>; length so far <{2}>", calls, lines, length);
                    }
                }
            }

            foreach (var kvp in dateCallCounts)
            {
                Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
            }
            foreach (var kvp in appCallCounts)
            {
                Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
            }

            Console.WriteLine("Result\r\n Calls : <{0}> \r\n Lines : <{1}> \r\n Length : <{2}>", calls, lines, length);
            return;
        }
    }
}
