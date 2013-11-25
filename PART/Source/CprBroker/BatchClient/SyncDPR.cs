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
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Providers.DPR;

namespace BatchClient
{
    public class SyncDPR : ConsoleEnvironment
    {
        UuidCache UuidCache = new UuidCache();
        BrokerContext brokerContext;
        public override string[] LoadCprNumbers()
        {
            Utilities.UpdateConnectionString(this.BrokerConnectionString);
            BrokerContext.Initialize(this.ApplicationToken, "");
            brokerContext = BrokerContext.Current;

            string[] ret = null;

            if (string.IsNullOrEmpty(SourceFile))
            {
                using (var dataContext = new DPRDataContext(OtherConnectionString))
                {
                    ret = dataContext.PersonTotals.Select(t => t.PNR).ToArray().Select(pnr => pnr.ToPnrDecimalString()).ToArray();
                }
            }
            else
            {
                ret = Utilities.LoadCprNumbersOneByOne(SourceFile);
            }

            UuidCache.PreLoadExistingMappings();
            return ret;
        }

        public override void ProcessPerson(string pnr)
        {
            BrokerContext.Current = brokerContext;
            try
            {
                CprBroker.Engine.Local.Admin.LogFormattedSuccess("Converting person <{0}> from DPR",pnr);
                using (var dprDataContext = new DPRDataContext(OtherConnectionString))
                {
                    var pId = new PersonIdentifier() { CprNumber = pnr, UUID = UuidCache.GetUuid(pnr) };
                    var personInfo = PersonInfo.GetPersonInfo(dprDataContext, decimal.Parse(pnr));
                    var reg = personInfo.ToRegisteringType1(UuidCache.GetUuid, dprDataContext);
                    UpdateDatabase.UpdatePersonRegistration(pId, reg);
                }
            }
            catch (Exception ex)
            {
                Admin.LogException(ex);
                throw ex;
            }
        }
    }
}
