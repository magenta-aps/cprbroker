using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CprBroker.Utilities.ConsoleApps;

namespace BatchClient
{
    class PartClient : ConsoleEnvironment
    {
        public string[] LoadCprNumbersOneByOne()
        {
            var ret = new List<string>();
            var fileNames = SourceFile.Split(';');
            foreach (var fileName in fileNames)
            {
                string[] fileCprNumbers = File.ReadAllLines(fileName);

                ret = fileCprNumbers
                    .Select(cprNumber =>
                    {
                        cprNumber = cprNumber.Substring(0, Math.Min(10, cprNumber.Length));
                        while (cprNumber.Length < 10)
                        {
                            cprNumber = "0" + cprNumber;
                        }
                        if (!System.Text.RegularExpressions.Regex.Match(cprNumber, "\\A\\d{10}\\Z").Success)
                        {
                            throw new Exception("Invalid CPR number: " + cprNumber);
                        }
                        return cprNumber;
                    })
                    .Distinct()
                    .ToList();
            }
            return ret.ToArray();
        }

        public string[] LoadCprNumbersBatch()
        {
            var baseRet = LoadCprNumbersOneByOne();
            int batchSize = 500;
            var ret = new List<string>();
            var myRet = new List<string>(batchSize);
            for (int i = 0; i < baseRet.Length; i++)
            {
                myRet.Add(baseRet[i]);
                if (myRet.Count == batchSize || i == baseRet.Length - 1)
                {
                    ret.Add(string.Join(",", myRet.ToArray()));
                    myRet.Clear();
                }
            }
            return ret.ToArray();
        }
    }
}
