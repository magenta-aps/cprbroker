﻿/* ***** BEGIN LICENSE BLOCK *****
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
using System.Configuration;
using System.IO;

namespace BatchClient
{
    public class Utilities
    {
        public static void UpdateConnectionString(string brokerConnectionString)
        {
            CprBroker.Tests.PartInterface.Utilities.UpdateConnectionString(brokerConnectionString);
        }

        public static string[] LoadCprNumbersOneByOne(string sourceFile)
        {
            var ret = new List<string>();
            var fileNames = sourceFile.Split(';');
            foreach (var fileName in fileNames)
            {
                string[] fileCprNumbers = File.ReadAllLines(fileName);

                ret = fileCprNumbers
                    .Select(cprNumberOrUuid =>
                    {
                        if (CprBroker.Utilities.Strings.IsGuid(cprNumberOrUuid))
                        {
                            return cprNumberOrUuid;
                        }
                        else
                        {
                            cprNumberOrUuid = cprNumberOrUuid.Substring(0, Math.Min(10, cprNumberOrUuid.Length));
                            while (cprNumberOrUuid.Length < 10)
                            {
                                cprNumberOrUuid = "0" + cprNumberOrUuid;
                            }
                            if (!System.Text.RegularExpressions.Regex.Match(cprNumberOrUuid, "\\A\\d{10}\\Z").Success)
                            {
                                throw new Exception("Invalid CPR number: " + cprNumberOrUuid);
                            }
                            return cprNumberOrUuid;
                        }
                    })
                    .Distinct()
                    .ToList();
            }
            return ret.ToArray();
        }

        public static string[] LoadCprNumbersBatch(string sourceFile)
        {
            var baseRet = LoadCprNumbersOneByOne(sourceFile);
            int batchSize = 200;
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
