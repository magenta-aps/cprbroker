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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker.Subscriptions;

namespace CprBroker.EventBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service, Description = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Subscriptions : WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Subscribe, Description = CprBroker.Schemas.ServiceDescription.Subscriptions.Subscribe)]
        public BasicOutputType<ChangeSubscriptionType> Subscribe(ChannelBaseType NotificationChannel, Guid[] personUuids)
        {
            return SubscriptionManager.Subscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, personUuids);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Unsubscribe, Description = CprBroker.Schemas.ServiceDescription.Subscriptions.Unsubscribe)]
        public BasicOutputType<bool> Unsubscribe(Guid SubscriptionId)
        {
            return SubscriptionManager.Unsubscribe(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.SubscribeOnBirthdate, Description = CprBroker.Schemas.ServiceDescription.Subscriptions.SubscribeOnBirthdate)]
        public BasicOutputType<BirthdateSubscriptionType> SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] personUuids)
        {
            return SubscriptionManager.SubscribeOnBirthdate(applicationHeader.UserToken, applicationHeader.ApplicationToken, NotificationChannel, Years, PriorDays, personUuids);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.RemoveBirthDateSubscription, Description = CprBroker.Schemas.ServiceDescription.Subscriptions.RemoveBirthDateSubscriptions)]
        public BasicOutputType<bool> RemoveBirthDateSubscription(Guid SubscriptionId)
        {
            return SubscriptionManager.RemoveBirthDateSubscription(applicationHeader.UserToken, applicationHeader.ApplicationToken, SubscriptionId);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.GetActiveSubscriptionsList, Description = CprBroker.Schemas.ServiceDescription.Subscriptions.GetActiveSubscriptionsList)]
        public BasicOutputType<SubscriptionType[]> GetActiveSubscriptionsList()
        {
            return SubscriptionManager.GetActiveSubscriptionsList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Ping)]
        public BasicOutputType<bool> Ping()
        {
            return Engine.Ping.PingManager.Ping(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }
    }
}
