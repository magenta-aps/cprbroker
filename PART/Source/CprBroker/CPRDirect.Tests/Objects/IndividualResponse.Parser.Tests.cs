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
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace IndividualResponseTests
    {
        [TestFixture]
        public class ParseBatch
        {
            [Test]
            public void ParseAll___()
            {

                var rd = new System.IO.StringReader(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var dataLines = rd.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => l.Length >= 3);
                var allLineWrappers = dataLines.Select(line => new LineWrapper(line));

                var start = allLineWrappers.Where(w => w.IntCode == 0).First();
                var end = allLineWrappers.Where(w => w.IntCode == 999).First();

                allLineWrappers = allLineWrappers.Where(w => w.IntCode != 0 && w.IntCode != 999).ToArray();

                var groupedWrapers = allLineWrappers
                    .GroupBy(w => w.PNR);

                foreach (var group in groupedWrapers)
                {
                    var list = new List<LineWrapper>(group.ToList());
                    list.Insert(0, start);
                    list.Add(end);

                    var pnrLines = list.Select(w => w.Contents).ToArray();
                    var data = string.Join("\r\n", pnrLines.ToArray());
                }
            }

            [Test]
            public void ParseBatch_ChangeExtract_80Persons()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(80, result.Count);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllHasStartRecord(
                [Range(0, 79)]int index)
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.NotNull(result[index].StartRecord);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllShareStartRecord()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(1, result.Select(ind => ind.StartRecord).Distinct().Count());
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllHasEndRecord(
                [Range(0, 79)]int index)
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.NotNull(result[index].EndRecord);
            }

            [Test]
            public void ParseBatch_ChangeExtract_AllShareEndRecord()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                Assert.AreEqual(1, result.Select(ind => ind.EndRecord).Distinct().Count());
            }
        }
    }
}
