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
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web.Services
{

    /// <summary>
    /// Summary description for Part
    /// </summary>
    [WebService(Namespace = CprBroker.Schemas.Part.ServiceNames.Namespace, Name = CprBroker.Schemas.Part.ServiceNames.Part.Service, Description = CprBroker.Schemas.ServiceDescription.Part.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Part : PartService
    {
        public QualityHeader qualityHeader = new QualityHeader();
        private const string QualityHeaderName = "qualityHeader";

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Read, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.Read)]
        public LaesOutputType Read(LaesInputType input)
        {
            var localAction = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, localAction, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.RefreshRead, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.RefreshRead)]
        public LaesOutputType RefreshRead(LaesInputType input)
        {
            return PartManager.Read(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, SourceUsageOrder.ExternalOnly, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.List, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.List)]
        public ListOutputType1 List(ListInputType input)
        {
            var localAction = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.List(applicationHeader.UserToken, applicationHeader.ApplicationToken, input, localAction, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.Search, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.Search)]
        public SoegOutputType Search(SoegInputType1 searchCriteria)
        {
            return PartManager.Search(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, out qualityHeader.QualityLevel);
        }

        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        //[SoapHeader(QualityHeaderName, Direction = SoapHeaderDirection.Out)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.SearchList, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.Search)]
        public SoegListOutputType SearchList(SoegInputType1 searchCriteria)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.SearchList(applicationHeader.UserToken, applicationHeader.ApplicationToken, searchCriteria, sourceUsageOrder);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.GetUuid, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.GetUuid)]
        public GetUuidOutputType GetUuid(string cprNumber)
        {
            return PartManager.GetUuid(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumber);
        }

        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.Part.Methods.GetUuidArray, Description = CprBroker.Schemas.ServiceDescription.Part.Methods.GetUuidArray)]
        public GetUuidArrayOutputType GetUuidArray(string[] cprNumberArray)
        {
            return PartManager.GetUuidArray(applicationHeader.UserToken, applicationHeader.ApplicationToken, cprNumberArray);
        }

        [SoapHeader(ApplicationHeaderName)]
        public IBasicOutput<bool> PutSubscription(Guid[] personUuids)
        {
            return PartManager.PutSubscription(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUuids);
        }

        [WebMethod(Description = "Gets a single snapshot of one person's CPR data")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadSnapshot(LaesOejebliksbilledeInputType input)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.ReadAtTime(input, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets all snapshots of one person's CPR data that fall between the input effect dates")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public LaesOutputType ReadPeriod(LaesPeriodInputType input)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.ReadPeriod(input, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets a single snapshot of the given persons' CPR data")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListSnapshot(ListOejebliksbilledeInputType input)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.ListAtTime(input, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }

        [WebMethod(Description = "Gets all snapshots of the given persons' CPR data that fall between the input effect dates")]
        [SoapHeader(ApplicationHeaderName)]
        [SoapHeader(SourceUsageOrderHeaderName, Direction = SoapHeaderDirection.In)]
        public ListOutputType1 ListPeriod(ListPeriodInputType input)
        {
            var sourceUsageOrder = SourceUsageOrderHeader.GetLocalDataProviderUsageOption(this.sourceUsageOrderHeader);
            return PartManager.ListPeriod(input, applicationHeader.ApplicationToken, applicationHeader.UserToken, sourceUsageOrder);
        }
    }

}
