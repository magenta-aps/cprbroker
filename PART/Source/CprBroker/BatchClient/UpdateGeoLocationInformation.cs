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
 * Dennis Amdi Skov Isaksen
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
using CprBroker.Providers.DPR;
using CprBroker.DBR;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data;
using CprBroker.Data.Part;

namespace BatchClient
{
    class UpdateGeoLocationInformation : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            return new String[] { SourceFile };
        }

        public override void ProcessPerson(string pnr)
        {
            UpdateFromGeoLocationFile(SourceFile);
        }

        public void UpdateFromGeoLocationFile(string dataFile)
        {
            DateTime startTime = DateTime.Now;
            Log("Started at " + startTime);
            using (var dataStream = new FileStream(dataFile, FileMode.Open, FileAccess.Read))
            {
                CprConverter.ImportGeoInformationFileInSteps(dataStream, 20, Encoding.GetEncoding(1252), BrokerConnectionString);
            }
            DateTime endTime = DateTime.Now;
            TimeSpan diff = endTime.Subtract(startTime);
            var diffText = "Ended at " + DateTime.Now + "\nTotal time spent: " + diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds;
            Log(diffText);
        }

        public static void SplitFile()
        {
            var path = @"C:\Magenta Workspace\broker\PART\Doc\Data Providers\CPR Direct\Lookups\vejregister_hele_landet_pr_140801\A370715.txt";
            var streams = new Dictionary<string, StreamWriter>();

            using (var source = new System.IO.StreamReader(path, Encoding.GetEncoding(1252)))
            {
                while (!source.EndOfStream)
                {
                    int batchSize = 100;
                    var wrappers = CprBroker.Providers.CPRDirect.CompositeWrapper.Parse(source, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P05780, batchSize);

                    foreach (var w in wrappers)
                    {
                        var code = w.Contents.Substring(0, 3);
                        StreamWriter target;
                        if (streams.ContainsKey(code))
                        {
                            target = streams[code];
                        }
                        else
                        {
                            target = new StreamWriter(string.Format("{0}-{1}-{2}.txt", path, code, w.GetType().Name), false, Encoding.GetEncoding(1252));
                            streams[code] = target;
                        }
                        target.Write(w.Contents);
                    }
                }
            }
            foreach (var kvp in streams)
            {
                kvp.Value.Close();
            }
        }
    }
}