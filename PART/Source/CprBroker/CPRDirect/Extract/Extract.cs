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

namespace CprBroker.Providers.CPRDirect
{
    partial class Extract
    {
        public Extract(string batchFileText, Dictionary<string, Type> typeMap, Dictionary<string, bool> reverseRelationMap, string sourceFileName = "")
            : this()
        {
            var dataLines = new List<LineWrapper>(LineWrapper.ParseBatch(batchFileText));

            var startLine = dataLines.First();
            var endLine = dataLines.Last();

            dataLines.Remove(startLine);
            dataLines.Remove(endLine);

            this.ExtractId = Guid.NewGuid();
            this.Filename = sourceFileName;
            this.ExtractDate = (startLine.ToWrapper(typeMap) as StartRecordType).ProductionDate.Value;
            this.ImportDate = DateTime.Now;
            this.StartRecord = startLine.Contents;
            this.EndRecord = endLine.Contents;

            this.ExtractItems.AddRange(
                dataLines
                .Select(line =>
                    {
                        var ret = line.ToExtractItem(typeMap, reverseRelationMap);
                        ret.ExtractId = this.ExtractId;
                        return ret;
                    }
                ));
        }

        public static IndividualResponseType GetPerson(string pnr, IQueryable<ExtractItem> extractItems, Dictionary<string, Type> typeMap)
        {
            var found = extractItems
                .Where(item => item.PNR == pnr)
                .GroupBy(item => item.Extract)
                .OrderByDescending(g => g.Key.ExtractDate)
                .FirstOrDefault();

            if (found != null)
            {
                var individualResponse = new IndividualResponseType();

                var linewWappers = found
                    .Select(item => new LineWrapper(item.Contents).ToWrapper(typeMap))
                    .ToArray();

                // TODO: (Reverse relation) Add reversible relationship support after finding a good indexing solution

                var startWrapper = new LineWrapper(found.Key.StartRecord).ToWrapper(typeMap);
                var endWrapper = new LineWrapper(found.Key.EndRecord).ToWrapper(typeMap);
                individualResponse.FillFrom(linewWappers, startWrapper, endWrapper);

                return individualResponse;
            }

            return null;
        }
    }
}