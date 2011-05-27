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

namespace CprBroker.Engine
{
    public static partial class Manager
    {
        /// <summary>
        /// This part contains methods related to admin interface
        /// All methods here simply delegate the code to Manager.CallMethod&lt;&gt;()
        /// </summary>
        public class Admin
        {
            #region Versioning management
            public static BasicOutputType<ServiceVersionType[]> GetCapabilities(string userToken, string appToken)
            {
                var facade = new GetCapabilitiesFacadeMethod(appToken, userToken);
                return GetMethodOutput<ServiceVersionType[]>(facade);
            }
            public static BasicOutputType<bool> IsImplementing(string userToken, string appToken, string methodName, string version)
            {
                var facade = new IsImplementingFacadeMethod(appToken, userToken, methodName, version);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Provider list
            public static BasicOutputType<Schemas.DataProviderType[]> GetDataProviderList(string userToken, string appToken)
            {
                var facade = new DataProviders.GetDataProviderListFacadeMethodInfo(appToken, userToken);
                return GetMethodOutput<DataProviderType[]>(facade);
            }
            public static BasicOutputType<bool> SetDataProviderList(string userToken, string appToken, DataProviderType[] dataProviders)
            {
                var facade = new DataProviders.SetDataProvidersFacadeMethodInfo(dataProviders, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Application
            public static BasicOutputType<ApplicationType> RequestAppRegistration(string userToken, string appToken, string name)
            {
                var facade = new RequestAppRegistrationFacadeMethod(name, appToken, userToken);
                return GetMethodOutput<ApplicationType>(facade);
            }

            public static BasicOutputType<bool> ApproveAppRegistration(string userToken, string appToken, string targetAppToken)
            {
                var facade = new ApproveAppRegistrationFacadeMethod(targetAppToken, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }

            public static BasicOutputType<ApplicationType[]> ListAppRegistrations(string userToken, string appToken)
            {
                var facade = new ListAppRegistrationsFacadeMethod(appToken, userToken);
                return GetMethodOutput<ApplicationType[]>(facade);
            }

            public static BasicOutputType<bool> UnregisterApp(string userToken, string appToken, string targetAppToken)
            {
                var facade = new UnregisterAppFacadeMethod(targetAppToken, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

            #region Logging

            public static BasicOutputType<bool> Log(string userToken, string appToken, string text)
            {
                var facade = new LogFacadeMethod(text, appToken, userToken);
                return GetMethodOutput<bool>(facade);
            }
            #endregion

        }
    }
}
