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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public class LineWrapper
    {
        public string Contents { get; set; }

        public LineWrapper(string contents)
        {
            Contents = contents;
        }

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }

        public string PNR
        {
            get { return Contents.Substring(3, 10); }
        }

        public bool IsValid
        {
            get
            {
                return Contents.Trim().Length > 0
                    && Contents.Length >= Constants.DataObjectCodeLength;
            }
        }

        public Wrapper ToWrapper(Dictionary<string, Type> typeMap)
        {
            if (typeMap.ContainsKey(this.Code))
            {
                Type type = typeMap[this.Code];
                var wrapper = Utilities.Reflection.CreateInstance(type) as Wrapper;
                this.Contents = this.Contents.PadRight(wrapper.Length);
                wrapper.Contents = this.Contents;
                return wrapper;
            }
            return null;
        }

        public PersonRecordWrapper ToPersonRecordWrapper(Dictionary<string, Type> typeMap, IRegistrationInfo registration)
        {
            var wrapper = ToWrapper(typeMap) as PersonRecordWrapper;
            wrapper.Registration = registration;
            return wrapper;
        }

        public ExtractItem ToExtractItem(Guid extractId, Dictionary<string, Type> typeMap, Dictionary<string, bool> relationMap, Dictionary<string, bool> multiRelationMap)
        {
            string relationPNR = null;
            string relationPNR2 = null;
            if (relationMap.ContainsKey(this.Code) && relationMap[this.Code])
            {
                relationPNR = (this.ToWrapper(typeMap) as IRelationship).RelationPNR;
            }
            else if (multiRelationMap.ContainsKey(this.Code) && multiRelationMap[this.Code])
            {
                var pnrs = (this.ToWrapper(typeMap) as IMultipleRelationship).RelationPNRs;
                if (pnrs.Length > 0)
                {
                    relationPNR = pnrs[0];
                    if (pnrs.Length > 1)
                    {
                        relationPNR2 = pnrs[1];
                    }
                }
            }


            return new ExtractItem()
            {
                ExtractItemId = Guid.NewGuid(),
                ExtractId = extractId,
                PNR = this.PNR,
                RelationPNR = relationPNR,
                RelationPNR2 = relationPNR2,
                Contents = this.Contents,
                DataTypeCode = this.Code
            };
        }

        public static LineWrapper[] ParseBatch(string batchFileText)
        {
            var rd = new System.IO.StringReader(batchFileText);
            var dataLines = rd
                .ReadToEnd()
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => new LineWrapper(l))
                .Where(w => w.IsValid)
                .ToArray();
            return dataLines;
        }
    }
}
