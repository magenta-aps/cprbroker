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
 * Dennis Isaksen
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
using CprBroker.Data.Part;

namespace CprBroker.Engine.Part
{
    public class PutSubscriptionFacadeMethodInfo : GenericFacadeMethodInfo<bool>
    {
        public Guid[] PersonUuids;
        public PersonIdentifier[] PersonIdentifiers;

        public PutSubscriptionFacadeMethodInfo(Guid[] uuids, string appToken, string userToken)
            : base(appToken, userToken)
        {
            PersonUuids = uuids;
        }

        public override StandardReturType ValidateInput()
        {
            if (PersonUuids == null)
                return StandardReturType.NullInput();
            else
            {
                /*
                 * I'm not entirely sure if any single element is allowed to be empty, but not the entire set or if no element may be empty.
                 * Therefore the two different attempts.
                 */
                // Not any empty elements are allowed
                foreach (Guid person in PersonUuids)
                {
                    if (person == Guid.Empty || person == null)
                        return StandardReturType.NullInput();
                }
                // Random empty elements are allowed, but not the entire set
                int count = 0;
                foreach (Guid person in PersonUuids)
                {
                    if (person != null && person != Guid.Empty)
                        count++;
                }
                if (PersonIdentifiers.Length != count)
                    return StandardReturType.NullInput();
            }


            PersonIdentifiers = PersonMapping.GetPersonIdentifiers(PersonUuids);

            foreach (PersonIdentifier pi in PersonIdentifiers)
            {
                if (pi == null)
                    return StandardReturType.NullInput();
            }

            return StandardReturType.OK();
        }

        public override void Initialize()
        {
            this.SubMethodInfos = PersonIdentifiers
                .Select(
                pi => new SubMethodInfo<IPutSubscriptionDataProvider, bool>()
                {
                    FailIfNoDataProvider = true,
                    FailOnDefaultOutput = true,
                    LocalDataProviderOption = SourceUsageOrder.ExternalOnly,
                    Method = prov => prov.PutSubscription(pi),
                    UpdateMethod = null,
                })
                .ToArray();
        }
    }



}
