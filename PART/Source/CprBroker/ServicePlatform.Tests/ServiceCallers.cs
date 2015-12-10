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
            var path = string.Format("..\\..\\Resources\\{0}.{1}.Response.OK.xml", pnr, serviceInfo.Name);
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(retXml);
            doc.Save(path);
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

    [TestFixture]
    public class StamPlusCaller : ServiceCaller
    {
        [TestCaseSource(typeof(CprBroker.Tests.CPRDirect.Utilities), "PNRs")]
        public void GetResponse(string pnr)
        {
            base.GetResponse(pnr, ServiceInfo.StamPlus_Local);
        }
    }

    [TestFixture]
    public class Adresse4Caller : ServiceCaller
    {
        [TestCaseSource(typeof(CprBroker.Tests.CPRDirect.Utilities), "PNRs")]
        public void GetResponse(string pnr)
        {
            base.GetResponse(pnr, ServiceInfo.Adresse4_Local);
        }
    }
}
