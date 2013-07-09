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
using System.IO;

using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Providers.DPR;

namespace BatchClient
{
    class LoadAllDataFromDatabase : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            using (DPRDataContext dataContext = new DPRDataContext(OtherConnectionString))
            {
                var all = dataContext.PersonTotals.Select(t => t.PNR).ToArray().Select(p => p.ToPnrDecimalString()).ToList();
                var ret = new List<string>(all.Count);
                Random r = new Random();
                while (all.Count > 0)
                {
                    var index = r.Next(0, all.Count);
                    ret.Add(all[index]);
                    all.RemoveAt(index);
                }
                return ret.ToArray();
            }
        }

        public override void ProcessPerson(string pnr)
        {
            var decimalPnr = decimal.Parse(pnr);
            using (DPRDataContext dataContext = new DPRDataContext(OtherConnectionString))
            {
                var expressionPersonInfo = PersonInfo.GetPersonInfo(dataContext, decimalPnr);
                // UUID mapping
                var map = new Dictionary<string, Guid>();
                Func<string, Guid> func = (string cpr) =>
                {
                    if (!map.ContainsKey(cpr))
                    {
                        map[cpr] = Guid.NewGuid();
                    }
                    return map[cpr];
                };
                var prov = new DprDatabaseDataProvider() { ConfigurationProperties = new Dictionary<string, string>() };
                
                var xmlObj = expressionPersonInfo.ToRegisteringType1(func, dataContext, prov);
                WriteObject(pnr, xmlObj);

            }
        }
    }
}
