﻿/* ***** BEGIN LICENSE BLOCK *****
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

namespace CprBroker.Tests.DPR.PersonInfoTests
{
    [TestFixture]
    public class ConversionTests
    {
        public string ConnectionString
        {
            get
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(CprBroker.Config.ConfigManager.Current.Settings.CprBrokerConnectionString);
                builder.InitialCatalog = "DPR";
                return builder.ToString();
            }
        }

        private decimal[] _PNRs = null;
        public decimal[] PNRs
        {
            get
            {
                if (_PNRs == null)
                {
                    using (var dataContext = new CprBroker.Providers.DPR.DPRDataContext(ConnectionString))
                    {
                        Random r = new Random();
                        var arr = dataContext.PersonTotals.Select(t => t.PNR).Distinct().ToList();
                        _PNRs = new decimal[10];
                        for (int i = 0; i < _PNRs.Length; i++)
                        {
                            int index = r.Next(0, arr.Count);
                            _PNRs[i] = arr[index];
                            arr.RemoveAt(index);
                        }
                    }
                }
                return _PNRs;
            }
        }

        Dictionary<string, Guid> _Uuids = new Dictionary<string, Guid>();
        Guid GetUuid(string pnr)
        {
            System.Threading.Thread.Sleep(100);
            if (!_Uuids.ContainsKey(pnr))
            {
                _Uuids[pnr] = Guid.NewGuid();
            }
            return _Uuids[pnr];
        }

        [Test]
        [TestCaseSource("PNRs")]
        public void ToRelationListeType_NoErrors(decimal pnr)
        {
            using (var dataContext = new DPRDataContext(ConnectionString))
            {
                var pInfo = PersonInfo.GetPersonInfo(dataContext, pnr);
                pInfo.ToRelationListeType(GetUuid, dataContext);
            }
        }

        [Test]
        [TestCaseSource("PNRs")]
        public void ToRegistreingType1_NoErrors(decimal pnr)
        {
            using (var dataContext = new DPRDataContext(ConnectionString))
            {
                var pInfo = PersonInfo.GetPersonInfo(dataContext, pnr);
                pInfo.ToRelationListeType(GetUuid, dataContext);
            }
        }

    }
}
