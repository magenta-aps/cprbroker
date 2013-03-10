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
using CprBroker.EventBroker;
using CprBroker.EventBroker.Data;
using CprBroker.Engine;

namespace CprBroker.EventBroker.Subscriptions
{
    /// <summary>
    /// Facade method for Unsubscribe & RemoveBirthdateSubscription web methods
    /// </summary>
    public class UnsubscribeFacadeMethod : GenericFacadeMethodInfo<bool>
    {
        Guid SubscriptionId;
        Data.SubscriptionType.SubscriptionTypes SubscriptionType;

        public UnsubscribeFacadeMethod(Guid subscriptionId, Data.SubscriptionType.SubscriptionTypes subscriptionType, string appToken, string userToken)
            : base(appToken, userToken)
        {
            SubscriptionId = subscriptionId;
            SubscriptionType = subscriptionType;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider, bool>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption = SourceUsageOrder.LocalThenExternal,
                    Method= prov=>
                        SubscriptionType== CprBroker.EventBroker.Data.SubscriptionType.SubscriptionTypes.Birthdate?
                            prov.RemoveBirthDateSubscription(SubscriptionId)
                            : prov.Unsubscribe(SubscriptionId),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            if (SubscriptionId == Guid.Empty)
            {
                return StandardReturType.NullInput();
            }

            using (var dataContext = new Data.EventBrokerDataContext())
            {
                var subscription = (from sub in dataContext.Subscriptions
                                    where sub.SubscriptionId == SubscriptionId && sub.SubscriptionTypeId == (int)this.SubscriptionType
                                    select sub
                                    ).SingleOrDefault();

                if (subscription == null)
                {
                    return StandardReturType.InvalidUuid(SubscriptionId.ToString());
                }
            }
            return StandardReturType.OK();
        }

    }
}
