using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using PersonMasterTestClient.personmaster;

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
