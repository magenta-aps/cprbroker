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
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Engine.Part
{
    /// <summary>
    /// Facade method for List
    /// </summary>
    public class GetUuidArrayFacadeMethodInfo : BatchFacadeMethodInfo<IPartPersonMappingDataProvider, GetUuidArrayOutputType, string, string>
    {
        public GetUuidArrayFacadeMethodInfo(string[] inp, string appToken, string userToken)
            : base(appToken, userToken)
        {
            input = inp;
            this.InitializationMethod = new Action(InitializationMethod);
        }

        public override StandardReturType ValidateInput()
        {
            if (input == null || input.Length == 0)
            {
                return StandardReturType.NullInput();
            }

            foreach (var pnr in input)
            {
                if (string.IsNullOrEmpty(pnr))
                {
                    return StandardReturType.NullInput();
                }
            }

            var invalidPnrs = (from pnr in input where !PartInterface.Strings.IsValidPersonNumber(pnr) select pnr).ToArray();
            if (invalidPnrs.Length > 0)
            {
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, String.Join(",", invalidPnrs));
            }

            return StandardReturType.OK();
        }

        protected override BatchSubMethodInfo<IPartPersonMappingDataProvider, string, string> CreateMainSubMethod()
        {
            return new GetUuidArraySubMethodInfo(input);
        }

        public override string[] Aggregate(object[] results)
        {
            // TODO: Is this method used?
            return (this.SubMethodInfos[0] as GetUuidArraySubMethodInfo)
                .States.Select(s => s.Output).ToArray();
        }

    }


    public class GetUuidArraySubMethodInfo : BatchSubMethodInfo<IPartPersonMappingDataProvider, string, string>
    {

        public GetUuidArraySubMethodInfo(string[] inp)
            : base(inp)
        {
        }

        string GuidToString(Guid? uuid)
        {
            if (uuid.HasValue)
                return uuid.Value.ToString();
            return null;
        }

        Guid? StringToGuid(string s)
        {
            if (!string.IsNullOrEmpty(s))
                return new Guid(s);
            return null;
        }

        public override string[] Run(IPartPersonMappingDataProvider prov, string[] input)
        {
            return prov.GetPersonUuidArray(input).Select(g => GuidToString(g)).ToArray();
        }

        public override void InvokeUpdateMethod(string[] input, string[] output)
        {
            Local.UpdateDatabase.UpdatePersonUuidArray(input, output.Select(s => StringToGuid(s)).ToArray());
        }

        public override bool IsUpdatableResult(string[] result)
        {
            // TODO: In this used?
            return true;
        }

        public override bool IsValidResult(string[] result)
        {
            // TODO: In this used?
            return States.Where(s => !string.IsNullOrEmpty(s.Output)).FirstOrDefault() == null;
        }
    }
}
