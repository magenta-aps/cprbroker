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

namespace CprBroker.Tests.CPRDirect.Tools
{
    namespace ExtractTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void Constructor_Parse_CorrectCount()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);

                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);
                var extract = parseResult.ToExtract();
                var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
                Assert.AreEqual(lines.Length - 2, extractItems.Count);
            }

            [Test]
            public void Constructor_Parse_CorrectStart()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);

                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);
                var extract = parseResult.ToExtract();
                Assert.AreEqual(lines.First().Contents, extract.StartRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectEnd()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);

                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);
                var extract = parseResult.ToExtract();
                Assert.AreEqual(lines.Last().Contents, extract.EndRecord);
            }

            [Test]
            public void Constructor_Parse_CorrectReconstruction()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);

                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);
                var extract = parseResult.ToExtract();
                var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
                var result = extractItems.Select(i => i.Contents).ToList();
                result.Insert(0, extract.StartRecord);
                result.Add(extract.EndRecord);
                for (int i = 0; i < lines.Length; i++)
                {
                    Assert.AreEqual(wrappers[i], result[i]);
                }
            }
        }

        [TestFixture]
        public class GetPerson
        {
            [Test]
            public void GetPerson_PersonExists_Correct(
                [Range(1, 1098, 20)]int lineNumber)
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);
                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);

                var extract = parseResult.ToExtract("", true, 0);
                var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
                extract.ExtractItems.AddRange(extractItems);

                var pnr = lines[2].PNR;
                var person = Extract.GetPersonFromLatestExtract(pnr, extract.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                Assert.NotNull(person);
                Assert.AreEqual(pnr, person.PersonInformation.PNR);
            }

            [Test]
            public void GetPerson_PersonNotExists_Null(
                [ValueSource(typeof(Utilities), "RandomCprNumberStrings5")]string cprNumber)
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(l => l.ToWrapper(Constants.DataObjectMap).Contents).ToArray();
                var newText = string.Join("", wrappers);
                var parseResult = new ExtractParseResult(newText, Constants.DataObjectMap);

                var extract = parseResult.ToExtract("", true, 0);
                var extractItems = parseResult.ToExtractItems(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap);
                extract.ExtractItems.AddRange(extractItems);

                var person = Extract.GetPersonFromLatestExtract(cprNumber, extract.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                Assert.Null(person);
            }
        }
    }
}
