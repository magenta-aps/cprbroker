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
    public class ExtractParseResult
    {
        public List<Wrapper> Wrappers { get; private set; }
        public List<LineWrapper> Lines { get; private set; }
        public StartRecordType StartWrapper { get; private set; }
        public LineWrapper StartLine { get; private set; }
        public LineWrapper EndLine { get; private set; }
        public LineWrapper[] ErrorLines { get; private set; }

        public ExtractParseResult(string batchFileText, Dictionary<string, Type> dataObjectMap)
        {
            this.Wrappers = Wrapper.Parse(batchFileText, dataObjectMap);
            var wrappersAndLines = this.Wrappers.Select(w => new { Wrapper = w, Line = new LineWrapper(w.Contents) }).ToArray();

            // Isolate error lines
            this.ErrorLines = wrappersAndLines.Where(wl => wl.Wrapper is ErrorRecordType).Select(wl => wl.Line).ToArray();
            this.Lines = wrappersAndLines.Where(wl => !(wl.Wrapper is ErrorRecordType)).Select(wl => wl.Line).ToList();

            this.StartWrapper = this.Wrappers.First() as StartRecordType;
            this.StartLine = this.Lines.First();
            this.EndLine = this.Lines.Last();

            this.Lines.Remove(this.StartLine);
            this.Lines.Remove(this.EndLine);
        }

        public Extract ToExtract(string sourceFileName = "", bool ready = false)
        {
            return new Extract()
            {
                ExtractId = Guid.NewGuid(),
                Filename = sourceFileName,
                ExtractDate = this.StartWrapper.ProductionDate.Value,
                ImportDate = DateTime.Now,
                StartRecord = this.StartLine.Contents,
                EndRecord = this.EndLine.Contents,
                Ready = ready
            };
        }

        public List<ExtractItem> ToExtractItems(Guid extractId, Dictionary<string, Type> typeMap, Dictionary<string, bool> reverseRelationMap)
        {
            return this.Lines
               .Select(
                   line => line.ToExtractItem(extractId, typeMap, reverseRelationMap))
               .ToList();
        }

        public List<ExtractPersonStaging> ToExtractPersonStagings(Guid extractId)
        {
            return this.Lines
                .GroupBy(line => line.PNR)
               .Select(
                   g => new ExtractPersonStaging()
                   {
                       ExtractPersonStagingId = Guid.NewGuid(),
                       ExtractId = extractId,
                       PNR = g.Key
                   })
               .ToList();
        }

        public List<ExtractError> ToExtractErrors(Guid extractId)
        {
            return this.ErrorLines.Select(el => new ExtractError
            {
                ExtractErrorId = Guid.NewGuid(),
                ExtractId = extractId,
                Contents = el.Contents
            }).ToList();
        }

    }
}
