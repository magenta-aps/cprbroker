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
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Engine.Local;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    partial class Extract
    {
        public static IndividualResponseType ToIndividualResponseType(IGrouping<Extract, ExtractItem> found, Dictionary<string, Type> typeMap)
        {
            return ToIndividualResponseType(found.Key, found.AsQueryable(), typeMap);
        }


        private StartRecordType _StartWrapper;
        public StartRecordType StartWrapper
        {
            get { return _StartWrapper; }
        }

        partial void OnStartRecordChanged()
        {
            if (string.IsNullOrEmpty(StartRecord))
            {
                _StartWrapper = null;
            }
            else
            {
                _StartWrapper = new LineWrapper(StartRecord).ToWrapper(Constants.DataObjectMap) as StartRecordType;
            }
        }
        partial void OnLoaded()
        {
            OnStartRecordChanged();
            OnEndRecordChanged();
        }
        private EndRecordType _EndWrapper;
        public EndRecordType EndWrapper
        {
            get { return _EndWrapper; }
        }
        partial void OnEndRecordChanged()
        {
            if (string.IsNullOrEmpty(EndRecord))
            {
                _EndWrapper = null;
            }
            else
            {
                _EndWrapper = new LineWrapper(EndRecord).ToWrapper(Constants.DataObjectMap) as EndRecordType;
            }
        }

        public static IndividualResponseType ToIndividualResponseType(Extract extract, IQueryable<ExtractItem> extractItems, Dictionary<string, Type> typeMap)
        {
            var individualResponse = new IndividualResponseType();

            var personWrappers = extractItems
                .Select(item => Wrapper.Create(item.DataTypeCode, item.Contents, typeMap))
                .ToArray();

            // TODO: (Reverse relation) Add reversible relationship support after finding a good indexing solution
            individualResponse.FillPropertiesFromWrappers(personWrappers, extract.StartWrapper, extract.EndWrapper);
            individualResponse.SourceObject = extract.ExtractId;

            return individualResponse;
        }

        public static IndividualResponseType GetPersonFromLatestExtract(string pnr, IQueryable<ExtractItem> extractItems, Dictionary<string, Type> typeMap)
        {
            var found = extractItems
                .Where(item => item.PNR == pnr)
                .GroupBy(item => item.Extract)
                .Where(g => g.Key.Ready)
                .OrderByDescending(g => g.Key.ExtractDate)
                .FirstOrDefault();

            if (found != null)
            {
                return ToIndividualResponseType(found, typeMap);
            }
            return null;
        }

        public static IQueryable<IndividualResponseType> GetPersonFromAllExtracts(string pnr, IQueryable<ExtractItem> extractItems, Dictionary<string, Type> typeMap)
        {
            return extractItems
                .Where(item => item.PNR == pnr)
                .GroupBy(item => item.Extract)
                .Where(g => g.Key.Ready)
                .Select(g => ToIndividualResponseType(g, typeMap));
        }
    }
}