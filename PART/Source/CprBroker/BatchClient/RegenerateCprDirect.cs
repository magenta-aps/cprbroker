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
using System.Configuration;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Data.Part;
using CprBroker.Utilities;
using CprBroker.Providers.CPRDirect;

namespace BatchClient
{
    public class RegenerateCprDirect : RegenerateContents
    {
        public override Guid ActorId
        {
            get { return CprBroker.Providers.CPRDirect.Constants.ActorId; }
        }

        public override CprBroker.Schemas.Part.RegistreringType1 CreateXmlType(string pnr, PersonRegistration dbReg, Func<string, Guid> cpr2uuidFunc)
        {
            var sourceString = dbReg.SourceObjects.ToString();
            using (var conn = new System.Data.SqlClient.SqlConnection(this.BrokerConnectionString))
            {
                conn.Open();
            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
            Console.WriteLine("Setting connection string to : {0}", this.BrokerConnectionString);
            if (section == null)
            {
                section = new ConnectionStringsSection();
                config.Sections.Add("connectionString", section);
                config.Save();
            }
            var connStr = section.ConnectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"];
            if (connStr == null)
            {
                connStr = new ConnectionStringSettings("CprBroker.Config.Properties.Settings.CprBrokerConnectionString", this.BrokerConnectionString);
                section.ConnectionStrings.Add(connStr);
            }
            else
            {
                connStr.ConnectionString = this.BrokerConnectionString;
            }
            config.Save();
            Console.WriteLine("Setting connection saved");


            IndividualResponseType individualResponse;
            if (sourceString.StartsWith("<guid>"))
            {
                Console.WriteLine("By Extract: {0}", pnr);
                var extractId = Strings.Deserialize<Guid>(sourceString);
                Console.WriteLine("Extract Id: {0}", extractId);
                using (var extractDataContext = new ExtractDataContext(this.BrokerConnectionString))
                {
                    Console.WriteLine("Getting extract items");
                    var extractItems = extractDataContext.ExtractItems.Where(ei => ei.PNR == pnr && ei.ExtractId == extractId).ToArray();
                    Console.WriteLine("Getting person");
                    individualResponse = Extract.GetPerson(pnr, extractItems.AsQueryable(), CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                    Console.WriteLine("Person retrieved");
                }
            }
            else
            {
                Console.WriteLine("By TCP: {0}", pnr);
                var responseData = Strings.Deserialize<string>(sourceString);
                individualResponse = new IndividualResponseType() { Data = responseData };

                individualResponse.FillFrom(individualResponse.Data, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                individualResponse.SourceObject = individualResponse.Data;
            }
            DateTime effectDate = dbReg.BrokerUpdateDate;
            Console.WriteLine("Converting person");
            var reg = individualResponse.ToRegistreringType1(cpr2uuidFunc, effectDate);
            Console.WriteLine("Person converted");
            return reg;
        }
    }
}
