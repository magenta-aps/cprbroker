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
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data.Part;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CPRDirect;
using System.IO;

namespace BatchClient
{
    class ConvertCprDirectToFiles : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            Utilities.UpdateConnectionString(this.BrokerConnectionString);
            Console.WriteLine(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString);

            if (string.IsNullOrEmpty(SourceFile))
            {
                using (var dataContext = new ExtractDataContext())
                {
                    return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().OrderBy(pnr => pnr).ToArray();
                }
            }
            else
            {
                return Utilities.LoadCprNumbersOneByOne(SourceFile);
            }
        }

        Dictionary<string, Guid> _UUIDMap;

        public Guid GetUuid(string pnr)
        {
            if (_UUIDMap == null)
            {
                _UUIDMap = new Dictionary<string, Guid>();
                using (var dataContext = new PartDataContext())
                {
                    _UUIDMap = dataContext.PersonMappings.ToDictionary(pm => pm.CprNumber, pm => pm.UUID);
                }
            }
            if (!_UUIDMap.ContainsKey(pnr))
                _UUIDMap[pnr] = Guid.NewGuid();

            return _UUIDMap[pnr];
        }

        public override void ProcessPerson(string pnr)
        {
            /* Sample person in run "2013 05 27 14_12"
             * - First Egenskab has null start and end dates - shall the first start at birthdate?
             * - Married with unknown spouse PNR
             */

            using (var dataContext = new ExtractDataContext())
            {
                var extractItems = dataContext.ExtractItems.Where(ei => ei.PNR == pnr);
                var grouped = extractItems.GroupBy(ei => ei.Extract);

                var myOutDir = OutDir + pnr + "\\";
                Directory.CreateDirectory(myOutDir);

                var registrations = new List<RegistreringType1>();
                foreach (var extract in grouped.OrderBy(ex => ex.Key.ExtractDate))
                {
                    var resp = Extract.GetPersonFromLatestExtract(pnr, extract.AsQueryable(), CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                    var reg = resp.ToRegistreringType1(GetUuid);
                    registrations.Add(reg);
                    File.WriteAllText(
                        string.Format("{0}{1}.{2}.xml", myOutDir, pnr, reg.Tidspunkt.ToDateTime().Value.ToString("yyyyMMdd HHmm")),
                        CprBroker.Utilities.Strings.SerializeObject(reg)
                        );
                }
                var merged1 = RegistreringType1.Merge(new CprBroker.Schemas.PersonIdentifier() { UUID = GetUuid(pnr), CprNumber = pnr }, VirkningType.Create(DateTime.MinValue, DateTime.MaxValue), registrations.ToArray());
                File.WriteAllText(
                    string.Format("{0}{1}.All.1.xml", myOutDir, pnr),
                    CprBroker.Utilities.Strings.SerializeObject(merged1)
                    );

                var merged2 = new CPRDirectExtractDataProvider().ReadPeriod(DateTime.MinValue, DateTime.MaxValue, new CprBroker.Schemas.PersonIdentifier() { CprNumber = pnr, UUID = GetUuid(pnr) }, GetUuid);

                File.WriteAllText(
                    string.Format("{0}{1}.All.2.xml", myOutDir, pnr),
                    CprBroker.Utilities.Strings.SerializeObject(merged2)
                    );

                var effectDate = new DateTime(2013, 5, 1);
                var merged3 = new CPRDirectExtractDataProvider().ReadPeriod(effectDate, effectDate, new CprBroker.Schemas.PersonIdentifier() { CprNumber = pnr, UUID = GetUuid(pnr) }, GetUuid);

                File.WriteAllText(
                    string.Format("{0}{1}.All.3.xml", myOutDir, pnr),
                    CprBroker.Utilities.Strings.SerializeObject(merged3)
                    );



            }
        }
    }
}
