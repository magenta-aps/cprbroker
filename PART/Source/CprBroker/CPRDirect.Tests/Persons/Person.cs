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
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class Person
    {
        public string GetPNR()
        {
            return this.GetType().Name.Substring(2, 10);
        }

        public string GetData()
        {
            var t = typeof(Properties.Resources);
            var ret = t.InvokeMember(
                string.Format("PNR_3112970079", GetPNR()),
                 System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                 null,
                 null,
                 new object[0]
                 );
            return ret.ToString();
        }

        public IndividualResponseType GetPerson()
        {
            return GetPerson(GetPNR());
        }
        public static IndividualResponseType GetPerson(string pnr)
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            return all.Where(p => p.PersonInformation.PNR == pnr).First();
        }

        [Test]
        public static void Split()
        {
            var enc = Encoding.GetEncoding(1252);

            var txt = "";
            txt = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE;
            //txt = System.IO.File.ReadAllText(@"C:\Magenta Workspace\PART\Doc\Data Providers\CPR Direct\Test data\U12170-P opgavenr 110901 ADRNVN FE", enc);
            var dataLines = LineWrapper.ParseBatch(txt);
            var allWrappers = dataLines.ToList();

            var startRecord = allWrappers.Where(lw => lw.Code == "000").First();
            var endRecord = allWrappers.Where(lw => lw.Code == "999").First();

            allWrappers.Remove(startRecord);
            allWrappers.Remove(endRecord);

            var groupedWrapers = allWrappers
                .Where(w => w != null)
                .GroupBy(w => w.PNR)
                .ToList();

            foreach (var individualWrappersGrouping in groupedWrapers)
            {
                var individualLines = individualWrappersGrouping.ToList();
                var pnr = individualWrappersGrouping.First().PNR;
                var myLines = new List<LineWrapper>(individualWrappersGrouping);
                myLines.Insert(0, startRecord);
                myLines.Add(endRecord);
                var txtLines = myLines.Select(lw => lw.Contents);
                System.IO.File.WriteAllLines(
                    string.Format(@"..\..\Resources\PNR_{0}.txt", pnr),
                    txtLines.ToArray(),
                    enc
                    );
            }
        }
    }
}
