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
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;

namespace BatchClient
{
    class CompareExpressionToSimplified : ConsoleEnvironment
    {
        DateTime lastLog = DateTime.Now;
        void LogText(string text)
        {
            DateTime now = DateTime.Now;
            Log(string.Format("{0} {1}", (now - lastLog), text));
            lastLog = now;
        }

        public void GetPersonInfo_Normal_EqualsPersonInfoExpression(decimal pnr)
        {
            LogText("sarted");
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
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

                var simplifiedPersonInfo = PersonInfo.GetPersonInfo(dataContext, pnr);
                LogText("GetPersonInfo()");
                Assert.NotNull(simplifiedPersonInfo, "simplifiedPersonInfo");
                LogText("Assert");
                var simplifiedPersonRegistration = simplifiedPersonInfo.ToRegisteringType1(func, dataContext);
                LogText("Converted");
                Assert.NotNull(simplifiedPersonRegistration, "simplifiedPersonRegistration");
                LogText("Assert");
                var simplifiedXml = CprBroker.Utilities.Strings.SerializeObject(simplifiedPersonRegistration);
                LogText("Serialization");
                WriteObject(pnr.ToDecimalString(), simplifiedXml);
                LogText("WriteToFile");

                var expressionPersonInfo = PersonInfo.PersonInfoExpression.Compile()(dataContext).Where(pi => pi.PersonTotal.PNR == pnr).FirstOrDefault();
                LogText("Expression retrieval");
                if (expressionPersonInfo != null)
                {
                    var expressionPersonRegistration = expressionPersonInfo.ToRegisteringType1(func, dataContext);
                    LogText("Conversion");
                    Assert.NotNull(expressionPersonRegistration, "expressionPersonRegistration");
                    LogText("Assert");
                    var expressionXml = CprBroker.Utilities.Strings.SerializeObject(expressionPersonRegistration);
                    LogText("Serialization");
                    WriteObject(pnr.ToDecimalString() + "-expr", expressionXml);
                    LogText("WriteToFile");
                    Assert.AreEqual(expressionXml, simplifiedXml);
                    LogText("Assert");
                }
            }
            LogText("Done");
        }

        public override void ProcessPerson(string pnr)
        {
            decimal decimalPnr = decimal.Parse(pnr);
            GetPersonInfo_Normal_EqualsPersonInfoExpression(decimalPnr);
        }

        public override string[] LoadCprNumbers()
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext.PersonTotals.OrderBy(pt => pt.PNR).Select(pt => pt.PNR.ToString()).ToArray();
            }
        }

    }
}
