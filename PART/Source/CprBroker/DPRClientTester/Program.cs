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
//using CprBroker.Providers.DPR;
using System.IO;

namespace DPRClientTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //CallRead();
            RefreshData(args);
        }

        static string[] ReadCprNumbers(string[] args)
        {
            string fileName;            
            fileName = args[0];

            string[] allCprNumbers = File.ReadAllLines(fileName);
            for (int i = 0; i < allCprNumbers.Length; i++)
            {
                string cprNumber = allCprNumbers[i];
                cprNumber = cprNumber.Substring(0, Math.Min(10, cprNumber.Length));

                while (cprNumber.Length < 10)
                    cprNumber = "0" + cprNumber;
                if (!System.Text.RegularExpressions.Regex.Match(cprNumber, "\\A\\d{10}\\Z").Success)
                {
                    throw new Exception("Invalid CPR number: " + cprNumber);
                }
                allCprNumbers[i] = cprNumber;
            }
            Console.WriteLine(string.Format("Found {0} Persons", allCprNumbers.Length));
            return allCprNumbers;
        }

        static void SetStartNumber(string [] args, ref bool runOnAllNumbers, ref string startCprNumber)
        {
            runOnAllNumbers = true;
            if(args.Length >1)
            {
                string cprNumber = args[1];
                Console.WriteLine(string.Format("Found PNR argument {0}",cprNumber));
                
                if (System.Text.RegularExpressions.Regex.Match(cprNumber, "\\A\\d{10}\\Z").Success)
                {
                    Console.WriteLine("Will ignore previous numbers");
                    startCprNumber = cprNumber;
                    runOnAllNumbers = false;
                }
            }
        }

        static void RefreshData(string[] args)
        {
            var partService = new DPRClientTester.Part.Part();
            partService.Url = "http://localhost/CprBroker/Services/Part.asmx";
            partService.ApplicationHeaderValue = new DPRClientTester.Part.ApplicationHeader() { ApplicationToken = "824fd432-b76e-4607-88e9-1cf8c71468aa", UserToken = "Beemen" };

            // TODO: If you need to run on only a part of the list, set 'startCprNumer' to a number to start with, and set 'runAllCprNumbers' = false
            string startCprNumer = "";
            bool runAllCprNumbers = true;

            string[] allCprNumbers = ReadCprNumbers(args);
            SetStartNumber(args, ref runAllCprNumbers, ref startCprNumer);

            for (int i = 0; i < allCprNumbers.Length; i++)
            {
                string cprNumber = allCprNumbers[i];

                if (!runAllCprNumbers)
                {
                    runAllCprNumbers = cprNumber == startCprNumer;
                }
                if (!runAllCprNumbers)
                {
                    Console.WriteLine(string.Format("{0} Ignoring", cprNumber));
                    continue;
                }

                var getUuidResult = partService.GetUuid(cprNumber);

                if (ValidateResult(cprNumber, "GetUuid", getUuidResult.StandardRetur))
                {
                    var uuid = getUuidResult.UUID;
                    var request = new DPRClientTester.Part.LaesInputType()
                    {                        
                        UUID = uuid
                    };
                    var readResult = partService.RefreshRead(request);
                    if (ValidateResult(cprNumber, "Read", readResult.StandardRetur))
                    {
                        Console.WriteLine(string.Format("{0} Succeeded", cprNumber));
                        File.AppendAllText("Success.txt", string.Format("{0}{1}", cprNumber, Environment.NewLine));
                    }
                }
            }
        }

        static bool ValidateResult(string cprNumber, string methodName, DPRClientTester.Part.StandardReturType standardRetur)
        {
            int statusCode;
            if (int.TryParse(standardRetur.StatusKode, out statusCode) && statusCode == 200)
            {
                return true;
            }
            else
            {
                string error = string.Format("{0} {1} {2} {3}", cprNumber, methodName, standardRetur.StatusKode, standardRetur.FejlbeskedTekst);
                Console.WriteLine(error);
                File.AppendAllText("Errors.txt", error + Environment.NewLine);
                return false;
            }
        }

        /*
        static void CallRead()
        {
            string cprNumber = "1006510015";
            var prov = new DprDatabaseDataProvider();
            //var ret = prov.Read2(cprNumber);

             public RegistreringType1 Read2(string cprNumber)
            {
                RegistreringType1 ret = null;
                using (var dataContext = new DPRDataContext("Data source=10.20.1.20; database=dpr; user id=sa; password=Dlph10t"))
                {
                    System.IO.StreamWriter w = new System.IO.StreamWriter(string.Format("C:\\Logs\\{0}{1}.sql",cprNumber,DateTime.Now.ToString("YYYY-MM-DD HH-mm-ss")));
                    w.AutoFlush = true;
                    dataContext.Log = w;
                    var db =
                    (
                        from personInfo in PersonInfo.PersonInfoExpression.Compile()(dataContext)
                        where personInfo.PersonTotal.PNR == Decimal.Parse(cprNumber)
                        select personInfo
                    ).FirstOrDefault();
                }
                    
                return ret;
            }
        }
        */
    }
}
