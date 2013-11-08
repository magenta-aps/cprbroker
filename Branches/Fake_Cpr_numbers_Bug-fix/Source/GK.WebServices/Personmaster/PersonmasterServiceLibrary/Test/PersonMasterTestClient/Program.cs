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
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using PersonMasterTestClient.PersonMasterServiceLibrary;

namespace PersonMasterTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] counts = new int[] { 500, 600, 700, 800, 850, 851};
            foreach (var count in counts)
            {
                Console.WriteLine(string.Format("Attempting with count = {0}", count));
                SqlDataAdapter adpt = new SqlDataAdapter(
                    string.Format("select top {0} cprNo from T_PM_CPR", count),
                    "");

                DataTable table = new DataTable();
                int found = adpt.Fill(table);
                Console.WriteLine(string.Format("Found = {0}", found));
                DataRow[] rows = new DataRow[found];
                table.Rows.CopyTo(rows, 0);
                string[] cprNumbers = (from DataRow dr in rows
                                       select dr["cprNo"].ToString()).ToArray();


                WSHttpBinding binding = new WSHttpBinding();
                string SpnName = "syddjurs.dk";
                string Address = "http://personmaster-service/PersonmasterServiceLibrary.BasicOp.svc";

                var identity = new SpnEndpointIdentity(SpnName);
                EndpointAddress endPointAddress = new EndpointAddress(new Uri(Address + "/PersonMasterService12"), identity);
                BasicOpClient client = new BasicOpClient(binding, endPointAddress);

                string aux = "";
                try
                {
                    client.GetObjectIDsFromCprArray("", cprNumbers, ref aux);
                    Console.WriteLine("Succeeded !!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.WriteLine(string.Format("Aux={0}", aux));
                }
            }
        }
    }
}
