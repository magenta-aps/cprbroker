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
        public TService CreateService<TInterface, TService>(string url)
            where TService : System.ServiceModel.ClientBase<TInterface>, TInterface, new()
            where TInterface: class
        {
            // Binding
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            // End point
            var endPointAddress = new EndpointAddress(url);

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

        public Kvit CallGctpService(string serviceUuid, string gctpMessage, out string retXml)
        {
            // Change message
            var doc = new XmlDocument();
            doc.LoadXml(gctpMessage);
            gctpMessage = doc.DocumentElement.OuterXml + Environment.NewLine;

            var service = CreateService<CprServicePortType, CprServicePortTypeClient>(this.Url);
            
            using (var callContext = this.BeginCall(serviceUuid, serviceUuid))
            {
                var invocationContext = GetInvocationContext<CprReplica.InvocationContextType>(serviceUuid);
                
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
            where TInvocationContext: IInvocationContext, new()
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
