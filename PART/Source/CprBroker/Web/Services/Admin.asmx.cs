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
using CprBroker.Engine;

namespace CprBroker.Web.Services
{
    /// <summary>
    /// Contains web methods related to administration of the system
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = ServiceNames.Admin.Service, Description = CprBroker.Schemas.ServiceDescription.Admin.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Admin : WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        #region Application manager
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.RequestAppRegistration, Description = CprBroker.Schemas.ServiceDescription.Admin.RequestAppRegistration)]
        public BasicOutputType<ApplicationType> RequestAppRegistration(string ApplicationName)
        {
            return Manager.Admin.RequestAppRegistration(Utilities.Constants.UserToken, Utilities.Constants.BaseApplicationToken.ToString(), ApplicationName);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.ApproveAppRegistration, Description = CprBroker.Schemas.ServiceDescription.Admin.ApproveAppRegistration)]
        public BasicOutputType<bool> ApproveAppRegistration(string ApplicationToken)
        {
            return Manager.Admin.ApproveAppRegistration(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.ListAppRegistrations, Description = CprBroker.Schemas.ServiceDescription.Admin.ListAppRegistrations)]
        public BasicOutputType<ApplicationType[]> ListAppRegistrations()
        {
            return Manager.Admin.ListAppRegistrations(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.UnregisterApp, Description = CprBroker.Schemas.ServiceDescription.Admin.UnregisterApp)]
        public BasicOutputType<bool> UnregisterApp(string ApplicationToken)
        {
            return Manager.Admin.UnregisterApp(applicationHeader.UserToken, applicationHeader.ApplicationToken, ApplicationToken);
        }
        #endregion

        #region Versioning
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(
            MessageName = ServiceNames.Admin.MethodNames.GetCapabilities,
            Description = CprBroker.Schemas.ServiceDescription.Admin.GetCapabilities)]
        public BasicOutputType<ServiceVersionType[]> GetCapabilities()
        {
            return Manager.Admin.GetCapabilities(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.IsImplementing, Description = CprBroker.Schemas.ServiceDescription.Admin.IsImplementing)]
        public BasicOutputType<bool> IsImplementing(string serviceName, string serviceVersion)
        {
            return Manager.Admin.IsImplementing(applicationHeader.UserToken, applicationHeader.ApplicationToken, serviceName, serviceVersion);
        }
        #endregion

        #region Provier list
        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.GetDataProviderList, Description = CprBroker.Schemas.ServiceDescription.Admin.GetDataProviderList)]
        public BasicOutputType<DataProviderType[]> GetDataProviderList()
        {
            return Manager.Admin.GetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.SetDataProviderList, Description = CprBroker.Schemas.ServiceDescription.Admin.SetDataProviderList)]
        public BasicOutputType<bool> SetDataProviderList(DataProviderType[] DataProviders)
        {
            return Manager.Admin.SetDataProviderList(applicationHeader.UserToken, applicationHeader.ApplicationToken, DataProviders);
        }
        #endregion

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = ServiceNames.Admin.MethodNames.Log, Description = CprBroker.Schemas.ServiceDescription.Admin.Log)]
        public BasicOutputType<bool> Log(string Text)
        {
            return Manager.Admin.Log(applicationHeader.UserToken, applicationHeader.ApplicationToken, Text);
        }

        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Subscriptions.Methods.Ping)]
        public BasicOutputType<bool> Ping()
        {
            return Engine.Ping.PingManager.Ping(Utilities.Constants.UserToken, Utilities.Constants.BaseApplicationToken.ToString());
        }
    }
}
