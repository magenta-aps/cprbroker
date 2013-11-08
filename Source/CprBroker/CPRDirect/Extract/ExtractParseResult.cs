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
        public List<LineWrapper> Lines { get; private set; }
        public StartRecordType StartWrapper { get; private set; }
        public LineWrapper EndLine { get; set; }
        public List<LineWrapper> ErrorLines { get; private set; }

        public enum WrapperParseGroup
        {
            Start,
            Regular,
            End,
            Error
        }

        public ExtractParseResult()
        {
            Lines = new List<LineWrapper>();
            ErrorLines = new List<LineWrapper>();
        }

        public ExtractParseResult(string batchFileText, Dictionary<string, Type> dataObjectMap)
            : this()
        {
            var wrappers = CompositeWrapper.Parse(batchFileText, dataObjectMap);
            AddLines(wrappers);
        }

        public static WrapperParseGroup GetParseGroup(Wrapper w)
        {
            if (w is StartRecordType)
                return WrapperParseGroup.Start;
            else if (w is EndRecordType)
                return WrapperParseGroup.End;
            else if (w is ErrorRecordType)
                return WrapperParseGroup.Error;
            else
                return WrapperParseGroup.Regular;
        }

        public void AddLines(List<Wrapper> wrappers)
        {
            var wrappersAndLines = wrappers.Select(w => new { Wrapper = w, Line = new LineWrapper(w.Contents), ParseGroupType = GetParseGroup(w)}).ToArray();

            // Isolate error lines
            this.ErrorLines.AddRange(wrappersAndLines.Where(wl => wl.ParseGroupType == WrapperParseGroup.Error).Select(wl => wl.Line));
            this.Lines.AddRange(wrappersAndLines.Where(wl => wl.ParseGroupType == WrapperParseGroup.Regular).Select(wl => wl.Line));

            var startWrapperAndLine = wrappersAndLines.Where(wl => wl.ParseGroupType == WrapperParseGroup.Start).FirstOrDefault();
            if (startWrapperAndLine != null)
            {
                this.StartWrapper = startWrapperAndLine.Wrapper as StartRecordType;
            }

            var endWrapperAndLine = wrappersAndLines.Where(wl => wl.ParseGroupType == WrapperParseGroup.End).FirstOrDefault();
            if (endWrapperAndLine != null)
            {
                this.EndLine = endWrapperAndLine.Line;
            }
        }

        public void ClearArrays()
        {
            this.Lines.Clear();
            this.ErrorLines.Clear();
        }

        public Extract ToExtract()
        {
            return ToExtract("");
        }

        public Extract ToExtract(string sourceFileName)
        {
            return ToExtract(sourceFileName, false, 0);
        }

        public Extract ToExtract(string sourceFileName, bool ready, long processedLines)
        {
            return new Extract()
            {
                ExtractId = Guid.NewGuid(),
                Filename = sourceFileName,
                ExtractDate = this.StartWrapper == null ? CprBroker.Utilities.Constants.MinSqlDate : this.StartWrapper.ProductionDate.Value,
                ImportDate = DateTime.Now,
                StartRecord = this.StartWrapper == null ? "" : this.StartWrapper.Contents,
                EndRecord = EndLine == null ? "" : this.EndLine.Contents,
                Ready = ready,
                ProcessedLines = processedLines
            };
        }

        public List<ExtractItem> ToExtractItems(Guid extractId, Dictionary<string, Type> typeMap, Dictionary<string, bool> relationMap, Dictionary<string, bool> multiRelationMap)
        {
            return this.Lines
               .Select(
                   line => line.ToExtractItem(extractId, typeMap, relationMap, multiRelationMap))
               .ToList();
        }

        public List<ExtractPersonStaging> ToExtractPersonStagings(Guid extractId)
        {
            return ToExtractPersonStagings(extractId, null);

        }

        public List<ExtractPersonStaging> ToExtractPersonStagings(Guid extractId, List<string> skipPnrs)
        {
            var pnrs = this.Lines
                .GroupBy(line => line.PNR)
                .Select(line => line.Key);
            if (skipPnrs != null)
            {
                pnrs = pnrs.Except(skipPnrs).ToArray();
                skipPnrs.AddRange(pnrs);
            }

            return pnrs
               .Select(
                   pnr => new ExtractPersonStaging()
                   {
                       ExtractPersonStagingId = Guid.NewGuid(),
                       ExtractId = extractId,
                       PNR = pnr
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
