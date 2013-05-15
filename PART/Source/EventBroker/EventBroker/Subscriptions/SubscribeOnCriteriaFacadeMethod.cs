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
using CprBroker.Engine;
using CprBroker.EventBroker.Data;
using CprBroker.EventBroker.Notifications;
using CprBroker.EventBroker.Subscriptions;
using CprBroker.Schemas.Part;

namespace CprBroker.EventBroker.Subscriptions
{
    class SubscribeOnCriteriaFacadeMethod : GenericFacadeMethodInfo<ChangeSubscriptionType>
    {
        ChannelBaseType NotificationChannel;
        SoegObjektType Criterion;

        public SubscribeOnCriteriaFacadeMethod(ChannelBaseType notificationChannel, SoegObjektType criterion, string appToken, string userToken)
            : base(appToken, userToken)
        {
            NotificationChannel = notificationChannel;
            Criterion = criterion;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[]
            {
                new SubMethodInfo<ISubscriptionDataProvider, ChangeSubscriptionType>()
                {
                    FailIfNoDataProvider=true,
                    FailOnDefaultOutput=true,
                    LocalDataProviderOption = SourceUsageOrder.LocalThenExternal,
                    Method= prov=>prov.SubscribeOnCriteria(NotificationChannel,Criterion),
                    UpdateMethod=null,
                }
            };
        }

        public override StandardReturType ValidateInput()
        {
            /*
             * Do the validation here
             */
            if (Criterion == null)
            {
                return StandardReturType.NullInput("SoegObjekt");
            }
            else
            {
                if (String.IsNullOrEmpty(Criterion.UUID))
                {
                    return StandardReturType.NullInput("SoegObjekt.UUID");
                }
                if (String.IsNullOrEmpty(Criterion.BrugervendtNoegleTekst))
                {
                    return StandardReturType.NullInput("SoegObjekt.BrugervendtNoegleTekst");
                }
            }
            /*
            if (Criterion.SoegObjekt.SoegVirkning != null)
            {
                return StandardReturType.InvalidValue("SoegObjekt.SoegVirkning", Criterion.SoegObjekt.SoegVirkning.ToString());
            }
             */
            if (Criterion.SoegAttributListe == null)
            {
                return StandardReturType.NullInput("SoegObjekt.SoegAttributListe");
            }
            else
            {
                if (Criterion.SoegAttributListe.SoegEgenskab == null)
                {
                    return StandardReturType.NullInput("SoegObjekt.SoegAttributListe.SoegEgenskab");
                }
                else
                {
                    int index = 0;
                    foreach (var prop in Criterion.SoegAttributListe.SoegEgenskab)
                    {
                        if (prop == null)
                        {
                            return StandardReturType.NullInput("SoegObjekt.SoegAttributListe.SoegEgenskab[" + index + "]");
                        }
                        else
                        {
                            if (prop.AndreAdresser != null)
                            {
                                CprBroker.Schemas.Part.DanskAdresseType address = (CprBroker.Schemas.Part.DanskAdresseType)prop.AndreAdresser.Item;
                                if (String.IsNullOrEmpty(address.AddressComplete.AddressAccess.MunicipalityCode))
                                {
                                    return StandardReturType.NullInput("SoegObjekt.SoegAttributListe.SoegEgenskab[" + index + "].AndreAdresser.AddressComplete.AddressAccess.MunicipalityCode");
                                }
                            }
                            else
                            {
                                return StandardReturType.NullInput("SoegObjekt.SoegAttributListe.SoegEgenskab[" + index + "].AndreAdresser");
                            }
                        }
                        index++;
                    }
                }
            }
            var channelValidationResult = ValidateChannel(NotificationChannel);
            if (!StandardReturType.IsSucceeded(channelValidationResult))
            {
                return channelValidationResult;
            }
            return StandardReturType.OK();
        }

        public static StandardReturType ValidateChannel(ChannelBaseType notificationChannel)
        {
            if (notificationChannel == null)
            {
                return StandardReturType.NullInput("NotificationChannel");
            }

            if (notificationChannel is WebServiceChannelType)
            {
                if (string.IsNullOrEmpty((notificationChannel as WebServiceChannelType).WebServiceUrl))
                {
                    return StandardReturType.NullInput("WebServiceUrl");
                }
            }
            else if (notificationChannel is FileShareChannelType)
            {
                if (string.IsNullOrEmpty((notificationChannel as FileShareChannelType).Path))
                {
                    return StandardReturType.NullInput("Path");
                }
            }
            else
            {
                return StandardReturType.UnknownObject("Unknown channel type");
            }

            var dbChannel = Data.Channel.FromXmlType(notificationChannel);
            if (dbChannel == null)
            {
                return StandardReturType.UnknownObject("NotificationChannel");
            }

            var channel = Notifications.Channel.Create(dbChannel);
            try
            {
                if (channel == null || !channel.IsAlive())
                {
                    // TODO: Call StandardReturType.UnreachableChannel()
                    return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Unreachable channel");
                }
            }
            catch (Exception ex)
            {
                Engine.Local.Admin.LogException(ex);
                // TODO: Call StandardReturType.UnreachableChannel()
                return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Unreachable channel");
            }
            return StandardReturType.OK();
        }
    }
}
