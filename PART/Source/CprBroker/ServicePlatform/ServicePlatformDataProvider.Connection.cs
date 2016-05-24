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
using CprBroker.Providers.ServicePlatform.CprReplica;
using CprBroker.Providers.CprServices;
using System.Net;
using CprBroker.Engine.Part;
using CprBroker.Engine;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider
    {
        public TService CreateService<TInterface, TService>(ServiceInfo serviceInfo)
            where TService : System.ServiceModel.ClientBase<TInterface>, TInterface, new()
            where TInterface : class
        {
            // Binding
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            binding.MaxBufferSize = 1024 * 1024 * 10;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            // Because default max 8192 is smaller than the returned GCTP message
            binding.ReaderQuotas.MaxStringContentLength = 1024 * 1024;

            // End point
            var uri = new Uri(this.Url);
            uri = new Uri(uri, serviceInfo.Path);
            var endPointAddress = new EndpointAddress(uri.ToString());

            // Create and initialize client
            var serviceType = typeof(TService);
            var service = serviceType.InvokeMember(
                null,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance,
                null,
                null,
                new object[]{
                    binding,endPointAddress}
                ) as TService;

            // Set credentials
            service.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySerialNumber, this.CertificateSerialNumber);

            return service;
        }

        public Kvit CallGctpService(ServiceInfo serviceInfo, string gctpMessage, out string retXml)
        {
            // Change message
            var doc = new XmlDocument();
            doc.LoadXml(gctpMessage);
            gctpMessage = doc.DocumentElement.OuterXml + Environment.NewLine;

            var service = CreateService<CprServicePortType, CprServicePortTypeClient>(serviceInfo);

            using (var callContext = this.BeginCall(serviceInfo.Name, serviceInfo.Name))
            {
                var invocationContext = GetInvocationContext<CprReplica.InvocationContextType>(serviceInfo.UUID);

                retXml = service.callGCTPCheckService(invocationContext, gctpMessage);
                var kvit = Kvit.FromResponseXml(retXml);
                if (kvit.OK)
                {
                    callContext.Succeed();
                }
                else
                {
                    callContext.Fail();
                }
                return kvit;
            }
        }

        public TInvocationContext GetInvocationContext<TInvocationContext>(string serviceUuid)
            where TInvocationContext : IInvocationContext, new()
        {
            return new TInvocationContext()
            {
                ServiceAgreementUUID = this.ServiceAgreementUuid,
                ServiceUUID = serviceUuid,
                UserSystemUUID = this.UserSystemUUID,
                UserUUID = this.UserUUID,
                OnBehalfOfUser = null,
                CallersServiceCallIdentifier = null,
                AccountingInfo = null
            };
        }
    }
}
