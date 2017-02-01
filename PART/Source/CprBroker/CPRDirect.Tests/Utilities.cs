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

namespace CprBroker.Tests.CPRDirect
{
    public static class Utilities
    {
        public static readonly Random Random = new Random();

        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char[] AlphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();

        public static char[] RandomChar(char[] arr, int count, params char[] exclude)
        {
            return RandomElement<char>(arr, count, exclude);
        }

        public static char RandomSingleChar(char[] arr, params char[] exclude)
        {
            return RandomChar(arr, 1, exclude).First();
        }

        public static T[] RandomElement<T>(T[] arr, int count, params T[] exclude)
        {
            var ret = new List<T>();
            while (ret.Count < count)
            {
                int index = Random.Next(0, arr.Length);
                T val = arr[index];
                if (!exclude.Contains(val) && !ret.Contains(val))
                    ret.Add(val);
            }
            return ret.ToArray();
        }

        public static string RandomCprNumberString()
        {
            return Converters.ToPnrStringOrNull(Utilities.RandomCprNumber().ToString());
        }

        public static decimal RandomCprNumber()
        {
            return decimal.Parse(CprBroker.Tests.PartInterface.Utilities.RandomCprNumber());
        }

        public static decimal[] RandomCprNumbers(int count)
        {
            var cprNumbers = new List<decimal>();

            for (int i = 0; i < count; i++)
            {
                cprNumbers.Add(RandomCprNumber());
            }
            return cprNumbers.ToArray();
        }

        public static string[] RandomCprNumberStrings(int count)
        {
            var cprNumbers = new List<string>();

            for (int i = 0; i < count; i++)
            {
                cprNumbers.Add(RandomCprNumberString());
            }
            return cprNumbers.ToArray();
        }

        public static string[] RandomCprNumberStrings5()
        {
            return RandomCprNumberStrings(5);
        }

        public static string[] PNRs = null;
        public static Dictionary<string, LineWrapper[]> PersonLineWrappers = null;
        public static Dictionary<string, ExtractItem[]> PersonExtractItems = null;
        public static ExtractItem[] AllExtractItems = null;

        public static LineWrapper StartLine = null;
        public static LineWrapper EndLine = null;
        
        public static Dictionary<string, IndividualResponseType> PersonIndividualResponses = null;

        static Utilities()
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            PNRs = all.Select(p => p.PersonInformation.PNR).OrderBy(p => p).ToArray();


            var lines = new List<LineWrapper>(LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE));

            StartLine = lines.First();
            EndLine = lines.Last();

            var personLines = lines.ToList();
            personLines.Remove(StartLine);
            personLines.Remove(EndLine);


            var groups = personLines.GroupBy(l => l.PNR).ToArray();

            PersonLineWrappers = groups.ToDictionary(g => g.Key, g => g.ToArray());

            var extract = new Extract() { ExtractId = Guid.NewGuid(), ExtractDate = DateTime.Now, StartRecord = StartLine.Contents, EndRecord = EndLine.Contents, Filename = "", ImportDate = DateTime.Now, Ready = true, ProcessedLines = lines.Count };
            var allItems = new List<ExtractItem>();
            PersonExtractItems = groups.ToDictionary(
                g => g.Key,
                g =>
                    {
                        var items = g.ToArray()
                            .Select(l => l.ToExtractItem(extract.ExtractId, Constants.DataObjectMap, Constants.RelationshipMap, Constants.MultiRelationshipMap))
                            .ToArray();
                        extract.ExtractItems.AddRange(items);
                        allItems.AddRange(items);
                        return items;
                    }
            );
            AllExtractItems = allItems.ToArray();

            PersonIndividualResponses = IndividualResponseType
                .ParseBatch(lines.ToArray(), Constants.DataObjectMap)
                .ToDictionary(o => o.PersonInformation.PNR);
        }
    }
}
