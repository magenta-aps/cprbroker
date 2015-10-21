using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using System.IO;

namespace CprBroker.Tests.ServicePlatform
{
    public class ServiceCaller
    {
        public void GetResponse(string pnr, ServiceInfo serviceInfo)
        {
            var prov = ServicePlatformDataProviderFactory.Create();
            var serviceMethod = serviceInfo.ToSearchMethod();
            var request = new SearchRequest(pnr);
            var plan = new SearchPlan(request, true, serviceMethod);
            var gctpMessage = plan.PlannedCalls.First().ToRequestXml(CprBroker.Providers.CprServices.Properties.Resources.SearchTemplate);
            string retXml = "";
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            var kvit = prov.CallGctpService(serviceInfo, gctpMessage, out retXml);
            Assert.True(kvit.OK, "GCTP failed");
            //"C:\MagentaWorkspace\broker.github\PART\Source\CprBroker\ServicePlatform.Tests\Resources\0101429059.Familie+.Response.OK.xml"
            var path = string.Format("..\\..\\Resources\\{0}.{1}.Response.OK.xml", pnr, serviceInfo.Name);
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(retXml);
            doc.Save(path);
            //File.WriteAllText(path, retXml, Encoding.UTF8);
        }
    }

    [TestFixture]
    public class FamilyPlusCaller : ServiceCaller
    {
        [TestCaseSource(typeof(CprBroker.Tests.CPRDirect.Utilities), "PNRs")]
        public void GetResponse(string pnr)
        {
            base.GetResponse(pnr, ServiceInfo.FamilyPlus_Local);
        }
    }
}
